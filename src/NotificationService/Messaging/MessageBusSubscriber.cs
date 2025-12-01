using System.Text;
using NotificationService.Messaging.EventProcessing;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using Shared.Constants;
using Shared.Extensions;

namespace NotificationService.Messaging;

public class MessageBusSubscriber : BackgroundService
{
    private readonly IEventProcessor _eventProcessor;
    private readonly IConfiguration _configuration;
    private readonly ILogger<MessageBusSubscriber> _logger;
    private IConnection _connection;
    private IChannel _channel;
    private string _queueName;
    
    private const int MaxRetryCount = 3;
    private readonly TimeSpan _retryInterval = TimeSpan.FromSeconds(1);
    private int _retryCounter;

    public MessageBusSubscriber(
        IEventProcessor eventProcessor,
        IConfiguration configuration,
        ILogger<MessageBusSubscriber> logger)
    {
        _eventProcessor = eventProcessor;
        _configuration = configuration;
        _logger = logger;
        InitializeRabbitMq();
    }

    private void InitializeRabbitMq()
    {
        var factory = new ConnectionFactory
        {
            HostName = _configuration["RabbitMq:HostName"]!,
            Port = 5672,
            UserName = _configuration["RabbitMq:User"]!,
            Password = _configuration["RabbitMq:Password"]!,
            RequestedHeartbeat = TimeSpan.FromSeconds(60),
        };

        _connection = factory
            .CreateConnectionAsync()
            .WaitAndGetResult();
        _channel = _connection
            .CreateChannelAsync()
            .WaitAndGetResult();
        _channel.ExchangeDeclareAsync(
            exchange: Exchange.DefaultExchange,
            type: ExchangeType.Direct,
            durable: true
        );

        _queueName = _channel
            .QueueDeclareAsync(
                queue: WellKnownNames.DefaultQueue,
                durable: true,
                exclusive: false
            )
            .WaitAndGetResult()
            .QueueName;

        BindRoutingKeys();

        _logger.LogInformation("Listening on the MessageBus");

        _connection.ConnectionShutdownAsync += (_, _) =>
        {
            _logger.LogInformation("RabbitMq connection shutdown");
            return Task.CompletedTask;
        };
    }

    private void BindRoutingKeys()
    {
        _channel.QueueBindAsync(
            queue: _queueName,
            exchange: Exchange.DefaultExchange,
            routingKey: RoutingKeys.ClientCreated
        ).WaitProperly();

        _channel.QueueBindAsync(
            queue: _queueName,
            exchange: Exchange.DefaultExchange,
            routingKey: RoutingKeys.SessionPlanned
        ).WaitProperly();

        _channel.QueueBindAsync(
            queue: _queueName,
            exchange: Exchange.DefaultExchange,
            routingKey: RoutingKeys.UserCreated
        ).WaitProperly();

        _channel.QueueBindAsync(
            queue: _queueName,
            exchange: Exchange.DefaultExchange,
            routingKey: RoutingKeys.UserLoggedIn
        ).WaitProperly();
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        stoppingToken.ThrowIfCancellationRequested();

        var consumer = new AsyncEventingBasicConsumer(_channel);
        consumer.ReceivedAsync += OnReceivedAsync;

        await _channel.BasicConsumeAsync(
            queue: _queueName,
            autoAck: false,
            consumer: consumer,
            cancellationToken: stoppingToken
        );
    }

    private async Task OnReceivedAsync(object model, BasicDeliverEventArgs ea)
    {
        _logger.LogInformation("Event received");

        try
        {
            var body = ea.Body;
            var route = ea.RoutingKey;
            var notificationMessage = Encoding.UTF8.GetString(body.ToArray());
            
            _eventProcessor.Process(route, notificationMessage);

            await _channel.BasicAckAsync(
                deliveryTag: ea.DeliveryTag,
                multiple: false,
                cancellationToken: ea.CancellationToken
            );
            _retryCounter = 0;
        }
        catch (Exception exception)
        {
            await ExecuteRetryLogicAsync(ea.DeliveryTag, exception, ea.CancellationToken);
        }
    }

    private async Task ExecuteRetryLogicAsync(ulong deliveryTag, Exception exception, CancellationToken ct)
    {
        _retryCounter++;
        var requeue = _retryCounter <= MaxRetryCount;
            
        await _channel.BasicRejectAsync(
            deliveryTag: deliveryTag,
            requeue: requeue,
            cancellationToken: ct
        );

        if (requeue)
        {
            _logger.LogError("Exception while message procession: {Message}", exception.GetBaseException().ToString());
            await Task.Delay(_retryInterval, ct);
        }
        else
        {
            _logger.LogCritical("Fatal error during processing a message. Message will be rejected. Exception message: {Message}", exception.GetBaseException().ToString());
            return;
        }
        _logger.LogInformation("Trying to retry message processing");
    }
}