using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using Microsoft.IdentityModel.Tokens;
using Shared.Constants;
using UserService.Extensions;
using UserService.Models;

namespace UserService.Helpers.Jwt;

public class JwtProvider : IJwtProvider
{
    private readonly IConfiguration _configuration;

    public JwtProvider(IConfiguration configuration)
    {
        _configuration = configuration;
    }

    public string GenerateJwtToken(User user)
    {
        Claim[] claims =
        [
            new(WellKnownNames.UserIdHeader, user.Id.ToString()),
            new(ClaimTypes.Name, user.UserName),
            new (ClaimTypes.Email, user.Email!)
        ];

        var securityKey = _configuration.GetSecurityKey();

        var credentials = new SigningCredentials(securityKey, SecurityAlgorithms.HmacSha256);
        var expiresHours = _configuration.GetValue<int>("JwtOptions:ExpiresHours");

        var token = new JwtSecurityToken(
            claims: claims,
            signingCredentials: credentials,
            expires: DateTime.Now.AddHours(expiresHours)
        );

        var tokenValue = new JwtSecurityTokenHandler().WriteToken(token);

        return tokenValue;
    }
}