using CRMService.Middlewares;

namespace CRMService.Extensions;

public static class WebApplicationExtensions
{
    public static WebApplication SetupMiddlewares(this WebApplication app)
    {
        app.UseExceptionHandling();
        app.MapControllers();
        app.UseSwagger();
        app.UseSwaggerUI();

        return app;
    }
}