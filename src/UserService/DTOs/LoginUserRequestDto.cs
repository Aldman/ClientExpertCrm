using System.ComponentModel.DataAnnotations;

namespace UserService.DTOs;

public class LoginUserRequestDto
{
    [Required]
    public required string Email { get; set; }
    [Required]
    public required string Password { get; set; }
}