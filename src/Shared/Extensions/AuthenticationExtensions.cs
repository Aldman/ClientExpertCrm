using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.IdentityModel.Tokens;
using Shared.Helpers;

namespace Shared.Extensions;

public static class AuthenticationExtensions
{
    public static IServiceCollection AddJwtAuthentication(
        this IServiceCollection services,
        string secretKey,
        string scheme = JwtBearerDefaults.AuthenticationScheme)
    {
        services.AddAuthentication(scheme)
            .AddJwtBearer(scheme, options =>
            {
                options.SaveToken = true;
                options.TokenValidationParameters = new TokenValidationParameters
                {
                    ValidateIssuer = false,
                    ValidateAudience = false,
                    ValidateLifetime = true,
                    ValidateIssuerSigningKey = true,
                    IssuerSigningKey = SecurityHelper.GetSecurityKey(secretKey),
                };
            });

        return services;
    }
}