namespace Shared.Events;

public class ClientCreatedEvent
{
    public required Guid ClientId { get; set; }
    public required string Name { get; set; }
    public required string Email { get; set; }
}