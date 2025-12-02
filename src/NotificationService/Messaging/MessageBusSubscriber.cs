using System.Text;
using NotificationService.Messaging.EventProcessing;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using Shared.Constants;

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
    
    private bool _isInitialized;

    public MessageBusSubscriber(
        IEventProcessor eventProcessor,
        IConfiguration configuration,
        ILogger<MessageBusSubscriber> logger)
    {
        _eventProcessor = eventProcessor;
        _configuration = configuration;
        _logger = logger;
    }

    private async Task InitializeRabbitMqIfNotAsync(CancellationToken ct = default)
    {
        if (_isInitialized) return;
        
        var factory = new ConnectionFactory
        {
            HostName = _configuration["RabbitMq:HostName"]!,
            Port = 5672,
            UserName = _configuration["RabbitMq:User"]!,
            Password = _configuration["RabbitMq:Password"]!,
            RequestedHeartbeat = TimeSpan.FromSeconds(60),
        };

        _connection = await factory.CreateConnectionAsync(ct);
        _channel = await _connection.CreateChannelAsync(cancellationToken: ct);
        await _channel.ExchangeDeclareAsync(
            exchange: Exchange.DefaultExchange,
            type: ExchangeType.Direct,
            durable: true,
            cancellationToken: ct
        );

        var queue = await _channel
            .QueueDeclareAsync(
                queue: WellKnownNames.DefaultQueue,
                durable: true,
                exclusive: false,
                cancellationToken: ct
            );
        _queueName = queue.QueueName;

        await BindRoutingKeysAsync(ct);

        _logger.LogInformation("Listening on the MessageBus");

        _connection.ConnectionShutdownAsync += (_, _) =>
        {
            _logger.LogInformation("RabbitMq connection shutdown");
            return Task.CompletedTask;
        };

        _isInitialized = true;
    }

    private async Task BindRoutingKeysAsync(CancellationToken ct = default)
    {
        await _channel.QueueBindAsync(
            queue: _queueName,
            exchange: Exchange.DefaultExchange,
            routingKey: RoutingKeys.ClientCreated,
            cancellationToken: ct
        );

        await _channel.QueueBindAsync(
            queue: _queueName,
            exchange: Exchange.DefaultExchange,
            routingKey: RoutingKeys.SessionPlanned,
            cancellationToken: ct
        );

        await _channel.QueueBindAsync(
            queue: _queueName,
            exchange: Exchange.DefaultExchange,
            routingKey: RoutingKeys.UserCreated,
            cancellationToken: ct
        );

        await _channel.QueueBindAsync(
            queue: _queueName,
            exchange: Exchange.DefaultExchange,
            routingKey: RoutingKeys.UserLoggedIn,
            cancellationToken: ct
        );
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        stoppingToken.ThrowIfCancellationRequested();
        
        await InitializeRabbitMqIfNotAsync(stoppingToken);

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