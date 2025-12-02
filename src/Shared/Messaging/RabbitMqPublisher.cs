using System.Text;
using System.Text.Json;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Polly.Registry;
using RabbitMQ.Client;
using Shared.Constants;
using Shared.Extensions;

namespace Shared.Messaging;

public class RabbitMqPublisher : IEventPublisher, IDisposable, IAsyncDisposable
{
    private readonly ILogger<RabbitMqPublisher> _logger;
    private readonly ResiliencePipelineProvider<string> _resilienceProvider;
    private readonly IConnection _connection;
    private readonly IChannel _channel;

    public RabbitMqPublisher(ILogger<RabbitMqPublisher> logger,
        ResiliencePipelineProvider<string> resilienceProvider,
        IConfiguration configuration)
    {
        _logger = logger;
        _resilienceProvider = resilienceProvider;

        var factory = new ConnectionFactory
        {
            HostName = configuration["RabbitMq:HostName"]!,
            Port = 5672,
            UserName = configuration["RabbitMq:User"]!,
            Password = configuration["RabbitMq:Password"]!,
            RequestedHeartbeat = TimeSpan.FromSeconds(60)
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
                durable: true,
                type: ExchangeType.Direct)
                .WaitProperly();
            _channel.QueueDeclareAsync(
                queue: WellKnownNames.DefaultQueue,
                exclusive: false,
                durable: true)
            .WaitProperly();

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
    
    public async Task PublishAsync<T>(T dto, 
        string routingKey, 
        CancellationToken cancellationToken = default) 
        where T : class
    {
        try
        {
            var message = JsonSerializer.Serialize(dto);
            var polly = _resilienceProvider.GetPipeline(WellKnownNames.RabbitMqRetrierName);

            if (_channel.IsOpen)
            {
                _logger.LogInformation("RabbitMq connection opened, sending message...");
                await polly.ExecuteAsync(async ct =>
                {
                    await SendMessageAsync(message, routingKey, ct);
                }, 
                    cancellationToken: cancellationToken);
            }
            else
            {
                _logger.LogInformation("RabbitMq connection closed, not sending.");
            }
        }
        catch (Exception e)
        {
            _logger.LogCritical("Could not publish due to an exception. Exception: {Message}", e.GetBaseException().Message);
        }
    }
    
    private async Task SendMessageAsync(string message, string routingKey, CancellationToken ct = default)
    {
        var body = Encoding.UTF8.GetBytes(message);
        
        await _channel.BasicPublishAsync(
            exchange: Exchange.DefaultExchange,
            routingKey: routingKey,
            body: body,
            cancellationToken: ct
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