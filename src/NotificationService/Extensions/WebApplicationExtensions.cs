namespace NotificationService.Extensions;

public static class WebApplicationExtensions
{
    public static WebApplication SetupMiddlewares(this WebApplication app)
    {
        app.UseSwagger();
        app.UseSwaggerUI();

        return app;
    }
}