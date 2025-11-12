using NotificationService.Extensions;
using Serilog;

Log.Logger = new LoggerConfiguration()
    .WriteTo.Console()
    .CreateBootstrapLogger();

try
{
    var builder = WebApplication.CreateBuilder(args);
    builder.Services.ConfigureAllServices();
    
    var app = builder
        .Build()
        .SetupMiddlewares();

    app.MapGet("/", () => "Hello World! From NotificationService");

    app.Run();
}
catch (Exception ex)
{
    Log.Fatal(ex, "Неожиданное завершение работы приложения. Текст ошибки: {ErrorMessage}", ex.Message);
}
finally
{
    Log.Information(Environment.NewLine);
    Log.CloseAndFlush();
}