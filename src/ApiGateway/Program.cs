using Serilog;

try
{
    Log.Logger = new LoggerConfiguration()
        .WriteTo.Console()
        .CreateBootstrapLogger();

    var builder = WebApplication.CreateBuilder(args);

    Log.Information("Starting configuring all services");
    builder.Services.AddControllers();
    builder.Services.AddSerilog((services, lc) => lc
        .ReadFrom.Configuration(builder.Configuration)
        .ReadFrom.Services(services));

    Log.Information("Setup app pipeline");
    var app = builder.Build();
    app.MapControllers();

    Log.Information("Starting app");
    app.Run();
}
catch (Exception ex)
{
    Log.Fatal(ex, "Unexpected shutdown of the application. Error text: {ErrorMessage}", ex.Message);
}
finally
{
    Log.CloseAndFlush();
}