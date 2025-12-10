namespace CRMService.DTOs.Client;

public class CreateClientRequestDto
{
    public required string Name { get; set; }
    public required string Email { get; set; }
    public required string Phone { get; set; }
}