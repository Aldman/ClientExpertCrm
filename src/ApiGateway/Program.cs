using Ocelot.DependencyInjection;
using Ocelot.Middleware;
using Serilog;
using Shared.Extensions;

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
    
    // todo: change it everywhere
    const string secretKey = "mysupersecret_secretsecretsecretkey!123";
    builder.Services.AddJwtAuthentication(secretKey);

    var envName = builder.Environment.EnvironmentName;
    Log.Information("Current environment: {EnvironmentName}", envName);
    builder.Configuration
        .SetBasePath(builder.Environment.ContentRootPath)
        .AddOcelot(primaryFile: $"ocelot.{envName}.json",
            optional: false,
            reloadOnChange: true
        );
    builder.Services.AddOcelot(builder.Configuration);

    Log.Information("Setup app pipeline");
    var app = builder.Build();
    app.UseAuthentication();
    app.UseAuthorization();
    await app.UseOcelot();

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