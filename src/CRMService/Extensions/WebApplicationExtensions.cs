using CRMService.Middlewares;

namespace CRMService.Extensions;

public static class WebApplicationExtensions
{
    public static WebApplication SetupMiddlewares(this WebApplication app)
    {
        app.MapControllers();
        app.UseSwagger();
        app.UseSwaggerUI();
        app.UseExceptionHandling();
        app.UseUserIdChecking();

        return app;
    }
}