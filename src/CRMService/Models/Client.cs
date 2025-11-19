namespace CRMService.Models;

public class Client
{
    public required Guid Id { get; set; }
    public required string Name { get; set; }
    public required string Email { get; set; }
    public required string Phone { get; set; }
    public required Guid UserId { get; set; }
    public required DateTime CreatedAt { get; set; } = DateTime.Now;
    
    public List<Session> Sessions { get; set; } = [];
}