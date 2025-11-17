using System.ComponentModel.DataAnnotations;

namespace UserService.DTOs;

public class RegisterUserRequestDto
{
    [Required]
    public required string Email { get; set; }
    [Required]
    public required string Password { get; set; }
    public string? UserName { get; set; }
}