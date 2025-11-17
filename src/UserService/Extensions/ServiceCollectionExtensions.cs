using Microsoft.EntityFrameworkCore;
using Shared.Constants;
using Shared.Messaging;
using UserService.Data;
using UserService.Data.Repository;
using UserService.Services;

namespace UserService.Extensions;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection ConfigureAllServices(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddDbContext<UsersDbContext>(options =>
        {
            AppContext.SetSwitch("Npgsql.EnableLegacyTimestampBehavior", true);
            AppContext.SetSwitch("Npgsql.DisableDateTimeInfinityConversions", true);
        
            var connectionString = configuration.GetConnectionString(WellKnownNames.DefaultConnection);
        
            options.UseNpgsql(connectionString);
        });
        services.AddScoped<IUserRepository, UserRepository>();
        services.AddScoped<IUserService, Services.UserService>();
        services.AddSingleton<IEventPublisher, RabbitMqPublisher>();
        services.AddControllers();
        services.AddSwaggerGen();

        return services;
    }
}