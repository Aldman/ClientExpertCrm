using System.Text;
using Microsoft.IdentityModel.Tokens;

namespace Shared.Helpers;

public static class SecurityHelper
{
    public static SymmetricSecurityKey GetSecurityKey(string secretKey)
    {
        var keyBytes = Encoding.UTF8.GetBytes(secretKey);
        return new SymmetricSecurityKey(keyBytes);
    }
}