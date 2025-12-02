using System.Text;
using System.Text.Json;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Polly.Registry;
using RabbitMQ.Client;
using Shared.Constants;

namespace Shared.Messaging;

public class RabbitMqPublisher : IEventPublisher, IDisposable, IAsyncDisposable
{
    private readonly ILogger<RabbitMqPublisher> _logger;
    private readonly ResiliencePipelineProvider<string> _resilienceProvider;
    private readonly IConfiguration _configuration;
    private IConnection _connection;
    private IChannel _channel;
    private bool _isInitialized;

    public RabbitMqPublisher(ILogger<RabbitMqPublisher> logger,
        ResiliencePipelineProvider<string> resilienceProvider,
        IConfiguration configuration)
    {
        _logger = logger;
        _resilienceProvider = resilienceProvider;
        _configuration = configuration;
    }

    private async Task InitializeConnectionIfNotAsync(CancellationToken ct = default)
    {
        if (_isInitialized) return;
        
        var factory = new ConnectionFactory
        {
            HostName = _configuration["RabbitMq:HostName"]!,
            Port = 5672,
            UserName = _configuration["RabbitMq:User"]!,
            Password = _configuration["RabbitMq:Password"]!,
            RequestedHeartbeat = TimeSpan.FromSeconds(60)
        };
        try
        {
            _connection = await factory.CreateConnectionAsync(ct);

            _channel = await _connection.CreateChannelAsync(cancellationToken: ct);
            await _channel.ExchangeDeclareAsync(
                exchange: Exchange.DefaultExchange,
                durable: true,
                type: ExchangeType.Direct,
                cancellationToken: ct
            );
            await _channel.QueueDeclareAsync(
                queue: WellKnownNames.DefaultQueue,
                exclusive: false,
                durable: true,
                cancellationToken: ct);

            _connection.ConnectionShutdownAsync += (_, _) =>
            {
                _logger.LogInformation("RabbitMq connection shutdown");
                return Task.CompletedTask;
            };
            _logger.LogInformation("Connected to Message Bus");
        }
        catch (Exception e)
        {
            _logger.LogError("Could not connect to the Message Bus. {Message}", e.Message);
            throw;
        }

        _isInitialized = true;
    }

    public async Task PublishAsync<T>(T dto,
        string routingKey,
        CancellationToken cancellationToken = default)
        where T : class
    {
        await InitializeConnectionIfNotAsync(cancellationToken);
        
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
            _logger.LogCritical("Could not publish due to an exception. Exception: {Message}",
                e.GetBaseException().Message);
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