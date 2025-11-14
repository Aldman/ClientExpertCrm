using NotificationService.Messaging;

namespace NotificationService.Extensions;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection ConfigureAllServices(this IServiceCollection services)
    {
        services.AddHostedService<MessageBusSubscriber>();
        services.AddControllers();
        services.AddSwaggerGen();

        return services;
    }
}