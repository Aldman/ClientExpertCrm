using NotificationService.Constants;
using NotificationService.Messaging;
using NotificationService.Messaging.EventProcessing;
using Polly;
using Polly.Retry;

namespace NotificationService.Extensions;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection ConfigureAllServices(this IServiceCollection services)
    {
        services.AddSingleton<IEventProcessor, EventProcessor>();
        services.AddHostedService<MessageBusSubscriber>();
        services.AddControllers();
        services.AddSwaggerGen();

        AddRabbitMqResilience(services);

        return services;
    }
    
    private static void AddRabbitMqResilience(IServiceCollection services)
    {
        services.AddResiliencePipeline(UsedNames.MessageBusSubscriberRetrierName, (builder, context) =>
        {
            var logger = context.ServiceProvider.GetRequiredService<ILogger<MessageBusSubscriber>>();

            builder.AddRetry(new RetryStrategyOptions
            {
                MaxRetryAttempts = 7,
                Delay = TimeSpan.FromMilliseconds(500),
                BackoffType = DelayBackoffType.Exponential,
                UseJitter = false,

                ShouldHandle = new PredicateBuilder()
                    .Handle<Exception>(),

                OnRetry = args =>
                {
                    logger.LogError("Could not connect to a message bus. Attempt number: {AttemptNumber}.",
                        args.AttemptNumber);

                    logger.LogInformation("Retry to connect");

                    return default;
                }
            });
        });
    }
}