using System.Text;
using Microsoft.IdentityModel.Tokens;

namespace UserService.Extensions;

public static class ConfigurationExtensions
{
    public static SymmetricSecurityKey GetSecurityKey(this IConfiguration configuration)
    {
        var key = configuration["JwtOptions:SecretKey"]!;
        var keyBytes = Encoding.UTF8.GetBytes(key);
        return new SymmetricSecurityKey(keyBytes);
    }
}