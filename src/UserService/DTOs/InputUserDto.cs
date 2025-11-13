using System.ComponentModel.DataAnnotations;

namespace UserService.DTOs;

public class InputUserDto
{
    [Required]
    public required string UserName { get; set; }
    [Required]
    public required string Password { get; set; }
    public string? Email { get; set; }
}