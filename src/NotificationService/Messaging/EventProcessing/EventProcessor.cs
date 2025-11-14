using Shared.Constants;

namespace NotificationService.Messaging.EventProcessing;

public class EventProcessor : IEventProcessor
{
    private readonly ILogger<EventProcessor> _logger;

    public EventProcessor(ILogger<EventProcessor> logger)
    {
        _logger = logger;
    }
    
    public void Process(string routingKey, string message)
    {
        if (routingKey == RoutingKeys.ClientCreated)
            _logger.LogInformation("Client created: {Message}", message);
        else
        {
            // todo: временный стаб
            _logger.LogInformation("Message: {Message}", message);
        }
    }
}