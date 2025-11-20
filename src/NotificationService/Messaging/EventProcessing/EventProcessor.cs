using Shared.Constants;
using Shared.Extensions;

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
                SendWelcomeMessage(message);
                break;
            case RoutingKeys.SessionPlanned:
                SendReminder(message);
                break;
        }
    }

    private void SendReminder(string message)
    {
        _logger.LogDebug("Sending reminder message: {Message}", message);
        
        var sessionPlannedEvent = message.ToSessionPlannedEvent();
        if (sessionPlannedEvent != null)
        {
            _logger.LogInformation("Reminder for session {date} ({minutes} min)",
                sessionPlannedEvent.ScheduledAt.ToString("dd.MM.yyyy HH:mm"),
                sessionPlannedEvent.DurationInMinutes
                );
        }
        else
            _logger.LogError("Incorrect client created message: {Message}", message);
    }

    private void SendWelcomeMessage(string message)
    {
        _logger.LogDebug("Sending welcome message: {Message}", message);
        
        var clientCreatedEvent = message.ToClientCreatedEvent();
        if (clientCreatedEvent != null)
        {
            _logger.LogInformation("Welcome email sent to {Email}", clientCreatedEvent.Email);
        }
        else
            _logger.LogError("Incorrect client created message: {Message}", message);
    }
}