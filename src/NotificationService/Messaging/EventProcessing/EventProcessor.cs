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
        switch (routingKey)
        {
            case RoutingKeys.UserCreated:
                _logger.LogInformation("User created: {Message}", message);
                break;
            case RoutingKeys.UserLoggedIn:
                _logger.LogInformation("User logged in: {Message}", message);
                break;
            case RoutingKeys.ClientCreated:
                _logger.LogInformation("Client created: {Message}", message);
                break;
            case RoutingKeys.SessionPlanned:
                _logger.LogInformation("Session planned: {Message}", message);
                break;
        }
    }
}