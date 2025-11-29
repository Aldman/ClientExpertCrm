using NotificationService.Messaging;
using NotificationService.Messaging.EventProcessing;

namespace NotificationService.Extensions;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection ConfigureAllServices(this IServiceCollection services)
    {
        services.AddSingleton<IEventProcessor, EventProcessor>();
        services.AddHostedService<MessageBusSubscriber>();
        services.AddControllers();
        services.AddSwaggerGen();

        return services;
    }
}