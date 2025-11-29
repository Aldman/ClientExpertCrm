namespace CRMService.DTOs.Session;

public class CreateSessionRequestDto
{
    public required Guid ClientId { get; set; }
    public required DateTime ScheduledAt { get; set; }
    public required int DurationInMinutes { get; set; }
    public required string Notes { get; set; }
}