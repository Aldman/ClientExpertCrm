using System.Text;
using System.Text.Json;
using Microsoft.Extensions.Logging;
using RabbitMQ.Client;
using Shared.Constants;
using Shared.Extensions;

namespace Shared.Messaging;

public class RabbitMqPublisher : IEventPublisher, IDisposable, IAsyncDisposable
{
    private readonly ILogger<RabbitMqPublisher> _logger;
    private readonly IConnection _connection;
    private readonly IChannel _channel;

    public RabbitMqPublisher(ILogger<RabbitMqPublisher> logger)
    {
        _logger = logger;

        var factory = new ConnectionFactory
        {
            HostName = "localhost"
        };
        try
        {
            _connection = factory
                .CreateConnectionAsync()
                .WaitAndGetResult();
            
            _channel = _connection
                .CreateChannelAsync()
                .WaitAndGetResult();
            _channel.ExchangeDeclareAsync(
                exchange: Exchange.DefaultExchange,
                type: ExchangeType.Direct);
            _channel.QueueDeclareAsync(
                queue: WellKnownNames.DefaultQueue,
                durable: true
            );

            _connection.ConnectionShutdownAsync += (_, _) =>
            {
                logger.LogInformation("RabbitMq connection shutdown");
                return Task.CompletedTask;
            };
            _logger.LogInformation("Connected to Message Bus");
        }
        catch (Exception e)
        {
            logger.LogError("Could not connect to the Message Bus. {Message}", e.Message);
            throw;
        }
    }
    
    public async Task PublishAsync<T>(T dto, string routingKey)
    {
        var message = JsonSerializer.Serialize(dto);

        if (_channel.IsOpen)
        {
            _logger.LogInformation("RabbitMq connection opened, sending message...");
            await SendMessage(message, routingKey);
        }
        else
        {
            _logger.LogInformation("RabbitMq connection closed, not sending.");
        }
    }
    
    public async Task SendMessage(string message, string routingKey)
    {
        var body = Encoding.UTF8.GetBytes(message);
        
        await _channel.BasicPublishAsync(
            exchange: Exchange.DefaultExchange,
            routingKey: routingKey,
            body: body
        );
        
        _logger.LogInformation("The message has been sent. Message: {Message}", message);
    }

    public void Dispose()
    {
        _connection.Dispose();
        _channel.Dispose();
    }

    public async ValueTask DisposeAsync()
    {
        await _connection.DisposeAsync();
        await _channel.DisposeAsync();
    }
}