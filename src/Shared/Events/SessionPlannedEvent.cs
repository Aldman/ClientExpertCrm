namespace Shared.Events;

public class SessionPlannedEvent
{
    public required Guid SessionId { get; set; }
    public required Guid ClientId { get; set; }
    public required DateTime ScheduledAt { get; set; }
    public required int DurationInMinutes { get; set; }
}