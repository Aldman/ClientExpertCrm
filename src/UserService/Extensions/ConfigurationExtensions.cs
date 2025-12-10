using Microsoft.IdentityModel.Tokens;
using Shared.Helpers;

namespace UserService.Extensions;

public static class ConfigurationExtensions
{
    public static SymmetricSecurityKey GetSecurityKey(this IConfiguration configuration)
    {
        var key = configuration["JwtOptions:SecretKey"]!;
        return SecurityHelper.GetSecurityKey(key);
    }
}