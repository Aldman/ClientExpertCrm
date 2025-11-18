using UserService.Models;

namespace UserService.Helpers.Jwt;

public interface IJwtProvider
{
    string GenerateJwtToken(User user);
}