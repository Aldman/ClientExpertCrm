using FluentValidation;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Serilog;
using Shared.Constants;
using Shared.Messaging;
using SharpGrip.FluentValidation.AutoValidation.Mvc.Extensions;
using UserService.Data;
using UserService.Data.Repository;
using UserService.DTOs;
using UserService.Helpers.Jwt;
using UserService.Services;
using UserService.Validators;

namespace UserService.Extensions;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection ConfigureAllServices(this IServiceCollection services, IConfiguration configuration)
    {
        ConfigureSerilog(services, configuration);
        ConfigureDbContext(services, configuration);
        services.AddScoped<IUserRepository, UserRepository>();
        services.AddScoped<IUserService, Services.UserService>();
        services.AddScoped<IJwtProvider, JwtProvider>();
        services.AddSingleton<IEventPublisher, RabbitMqPublisher>();
        services.AddControllers();
        services.AddSwaggerGen();
        ConfigureValidators(services);
        ConfigureAuth(services, configuration);

        return services;
    }

    private static void ConfigureValidators(IServiceCollection services)
    {
        services.AddFluentValidationAutoValidation();
        services.AddScoped<IValidator<RegisterUserRequestDto>, RegisterUserRequestDtoValidator>();
        services.AddScoped<IValidator<GetUsersRequest>, GetUsersRequestValidator>();
        services.AddScoped<IValidator<LoginUserRequestDto>, LoginUserRequestDtoValidator>();
    }

    private static void ConfigureAuth(IServiceCollection services, IConfiguration configuration)
    {
        Log.Information("Configuring Auth");
        
        services
            .AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
            .AddJwtBearer(options =>
            {
                options.TokenValidationParameters = new TokenValidationParameters
                {
                    ValidateIssuer = false,
                    ValidateAudience = false,
                    ValidateLifetime = true,
                    ValidateIssuerSigningKey = true,
                    IssuerSigningKey = configuration.GetSecurityKey(),
                };
                options.Events = new JwtBearerEvents
                {
                    OnMessageReceived = context =>
                    {
                        context.Token = context.Request.Cookies[WellKnownNames.TokenName];
                        return Task.CompletedTask;
                    }
                };
            });
        services.AddAuthentication();
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
        
        services.AddDbContext<UsersDbContext>(options =>
        {
            AppContext.SetSwitch("Npgsql.EnableLegacyTimestampBehavior", true);
            AppContext.SetSwitch("Npgsql.DisableDateTimeInfinityConversions", true);
        
            var connectionString = configuration.GetConnectionString(WellKnownNames.DefaultConnection);
        
            options.UseNpgsql(connectionString);
        });
    }
}