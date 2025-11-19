namespace CRMService.DTOs.Client;

public class UpdateClientRequestDto
{
    public required Guid Id { get; set; }
    public required string Name { get; set; }
    public required string Email { get; set; }
    public required string Phone { get; set; }
    public required Guid UserId { get; set; }
}