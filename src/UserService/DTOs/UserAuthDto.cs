namespace UserService.DTOs;

public class UserAuthDto
{
    public required Guid Id { get; set; }
    public required string UserName { get; set; }
    public string? Email { get; set; }
}