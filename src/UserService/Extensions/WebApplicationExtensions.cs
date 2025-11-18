using UserService.Middlewares;

namespace UserService.Extensions;

public static class WebApplicationExtensions
{
    public static WebApplication SetupMiddlewares(this WebApplication app)
    {
        app.MapControllers();
        app.UseSwagger();
        app.UseSwaggerUI();
        app.UseExceptionHandling();
        app.UseAuthentication();
        app.UseAuthorization();

        return app;
    }
}