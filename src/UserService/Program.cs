using Serilog;
using UserService.Extensions;

Log.Logger = new LoggerConfiguration()
    .WriteTo.Console()
    .CreateBootstrapLogger();

try
{
    var builder = WebApplication.CreateBuilder(args);
    builder.Services.ConfigureAllServices(builder.Configuration);
    
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
    Log.CloseAndFlush();
}