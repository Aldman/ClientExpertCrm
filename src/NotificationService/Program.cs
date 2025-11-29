using NotificationService.Extensions;
using Serilog;

Log.Logger = new LoggerConfiguration()
    .WriteTo.Console()
    .CreateBootstrapLogger();

try
{
    var builder = WebApplication.CreateBuilder(args);
    
    Log.Information("Starting configuring all services");
    builder.Services.ConfigureAllServices();
    builder.Services.AddSerilog((services, lc) => lc
        .ReadFrom.Configuration(builder.Configuration)
        .ReadFrom.Services(services));
    
    Log.Information("Setup app pipeline");
    var app = builder
        .Build()
        .SetupMiddlewares();

    Log.Information("Starting app");
    app.Run();
}
catch (Exception ex)
{
    Log.Fatal(ex, "Unexpected shutdown of the application. Error text: {ErrorMessage}", ex.Message);
}
finally
{
    Log.Information(Environment.NewLine);
    Log.CloseAndFlush();
}