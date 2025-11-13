namespace UserService.Extensions;

public static class WebApplicationExtensions
{
    public static WebApplication SetupMiddlewares(this WebApplication app)
    {
        app.MapControllers();
        app.UseSwagger();
        app.UseSwaggerUI();

        return app;
    }
}