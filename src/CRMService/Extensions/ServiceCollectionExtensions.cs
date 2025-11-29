using CRMService.Cache;
using CRMService.Constants;
using CRMService.Data;
using CRMService.Data.Repositories.Client;
using CRMService.Data.Repositories.Session;
using CRMService.DTOs.Client;
using CRMService.DTOs.Session;
using CRMService.Services.Client;
using CRMService.Services.Session;
using CRMService.Validators.Client;
using CRMService.Validators.Session;
using FluentValidation;
using Microsoft.EntityFrameworkCore;
using Serilog;
using Shared.Constants;
using Shared.Messaging;
using SharpGrip.FluentValidation.AutoValidation.Mvc.Extensions;
using StackExchange.Redis;

namespace CRMService.Extensions;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection ConfigureAllServices(
        this IServiceCollection services,
        IConfiguration configuration)
    {
        ConfigureSerilog(services, configuration);
        ConfigureDbContext(services, configuration);
        
        services.AddScoped<IClientRepository, ClientRepository>();
        services.AddScoped<ISessionRepository, SessionRepository>();
        services.AddScoped<ICacheRepository, CacheRepository>();
        services.AddScoped<IClientService, ClientService>();
        services.AddScoped<ISessionService, SessionService>();
        services.AddSingleton<IEventPublisher, RabbitMqPublisher>();
        services.AddSingleton<IConnectionMultiplexer>(_ =>
            ConnectionMultiplexer.Connect(configuration.GetConnectionString(RedisConstants.ConnectionStringName)!)
        );

        AddValidation(services);

        services.AddControllers();
        services.AddSwaggerGen();

        return services;
    }

    private static void AddValidation(IServiceCollection services)
    {
        services.AddFluentValidationAutoValidation();
        services.AddScoped<IValidator<CreateClientRequestDto>, CreateClientRequestValidator>();
        services.AddScoped<IValidator<UpdateClientRequestDto>, UpdateClientRequestValidator>();
        services.AddScoped<IValidator<CreateSessionRequestDto>, CreateSessionRequestValidator>();
        services.AddScoped<IValidator<UpdateSessionRequestDto>, UpdateSessionRequestValidator>();
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