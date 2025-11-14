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
    private readonly ILogger<MessageBusSubscriber> _logger;
    private IConnection _connection;
    private IChannel _channel;
    private string _queueName;

    public MessageBusSubscriber(
        IEventProcessor eventProcessor,
        ILogger<MessageBusSubscriber> logger)
    {
        _eventProcessor = eventProcessor;
        _logger = logger;
        InitializeRabbitMq();
    }
    
    private void InitializeRabbitMq()
    {
        var factory = new ConnectionFactory
        {
            HostName = "localhost",
            Port = 5672
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
                durable: true
                )
            .WaitAndGetResult()
            .QueueName;
        
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
        
        _logger.LogInformation("Listening on the MessageBus");

        _connection.ConnectionShutdownAsync += (_, _) =>
        {
            _logger.LogInformation("RabbitMq connection shutdown");
            return Task.CompletedTask;
        };
    }
    
    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        stoppingToken.ThrowIfCancellationRequested();

        var consumer = new AsyncEventingBasicConsumer(_channel);
        consumer.ReceivedAsync += (model, ea) =>
        {
            _logger.LogInformation("Event received");
            
            var body = ea.Body;
            var route = ea.RoutingKey;
            var notificationMessage = Encoding.UTF8.GetString(body.ToArray());
            
            _eventProcessor.Process(route, notificationMessage);
            return Task.CompletedTask;
        };

        await _channel.BasicConsumeAsync(
            queue: _queueName,
            autoAck: true,
            consumer: consumer,
            cancellationToken: stoppingToken
        );
    }
}