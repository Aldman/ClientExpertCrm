using NotificationService.Extensions;
using Serilog;

Log.Logger = new LoggerConfiguration()
    .WriteTo.Console()
    .CreateBootstrapLogger();

try
{
    var builder = WebApplication.CreateBuilder(args);
    builder.Services.ConfigureAllServices();
    builder.Services.AddSerilog((services, lc) => lc
        .ReadFrom.Configuration(builder.Configuration)
        .ReadFrom.Services(services));
    
    var app = builder
        .Build()
        .SetupMiddlewares();

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