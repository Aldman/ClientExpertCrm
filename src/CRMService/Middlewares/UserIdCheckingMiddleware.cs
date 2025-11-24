using CRMService.Exceptions;
using Shared.Constants;

namespace CRMService.Middlewares;

public class UserIdCheckingMiddleware
{
    private readonly RequestDelegate _next;

    public UserIdCheckingMiddleware(RequestDelegate next)
    {
        _next = next;
    }

    public async Task InvokeAsync(HttpContext httpContext)
    {
        if (httpContext.Request.Headers.TryGetValue(WellKnownNames.UserIdHeader, out var value))
        {
            if (Guid.TryParse(value, out _))
                await _next(httpContext);
            else
                throw new UserIdMissingException($"{WellKnownNames.UserIdHeader} is incorrect");
        }
        else
            throw new UserIdMissingException($"{WellKnownNames.UserIdHeader} header not found");
    }
}

public static class UserIdCheckingMiddlewareExtensions
{
    public static IApplicationBuilder UseUserIdChecking(this IApplicationBuilder app)
    {
        return app.UseMiddleware<UserIdCheckingMiddleware>();
    }
}