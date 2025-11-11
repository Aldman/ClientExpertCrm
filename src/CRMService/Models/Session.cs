namespace CRMService.Models;

public class Session
{
    public required Guid Id { get; set; }
    public required Guid ClientId { get; set; }
    public required DateTime ScheduledAt { get; set; }
    public required int DurationInMinutes { get; set; }
    public required Status Status { get; set; }
    public required string Notes { get; set; }
}

public enum Status
{
    Planned,
    Completed,
    Canceled,
}