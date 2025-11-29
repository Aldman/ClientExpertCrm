namespace NotificationService.Messaging.EventProcessing;

public interface IEventProcessor
{
    void Process(string routingKey, string message);
}