using CRMService.Data;
using Microsoft.EntityFrameworkCore;
using Serilog;
using Shared.Constants;

namespace CRMService.Extensions;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection ConfigureAllServices(
        this IServiceCollection services,
        IConfiguration configuration)
    {
        ConfigureSerilog(services, configuration);
        ConfigureDbContext(services, configuration);
        services.AddControllers();
        services.AddSwaggerGen();

        return services;
    }
    
    private static void ConfigureSerilog(IServiceCollection services, IConfiguration configuration)
    {
        services.AddSerilog((serviceProvider, loggerConfiguration) =>
        {
            loggerConfiguration
                .ReadFrom.Configuration(configuration)
                .ReadFrom.Services(serviceProvider);
        });
    }

    private static void ConfigureDbContext(IServiceCollection services, IConfiguration configuration)
    {
        Log.Information("Configuring DbContext");
        
        services.AddDbContext<CrmDbContext>(options =>
        {
            AppContext.SetSwitch("Npgsql.EnableLegacyTimestampBehavior", true);
            AppContext.SetSwitch("Npgsql.DisableDateTimeInfinityConversions", true);
        
            var connectionString = configuration.GetConnectionString(WellKnownNames.DefaultConnection);
        
            options.UseNpgsql(connectionString);
        });
    }
}