using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Polly;
using Polly.Retry;
using Shared.Constants;
using Shared.Messaging;

namespace Shared.Helpers;

public static class DiConfiguringHelper
{
    public static IServiceCollection AddRabbitMqResilience(IServiceCollection services)
    {
        services.AddResiliencePipeline(WellKnownNames.RabbitMqRetrierName, (builder, context) =>
        {
            var logger = context.ServiceProvider.GetRequiredService<ILogger<RabbitMqPublisher>>();

            builder.AddRetry(new RetryStrategyOptions
            {
                MaxRetryAttempts = 3,
                Delay = TimeSpan.FromMilliseconds(500),
                BackoffType = DelayBackoffType.Exponential,
                UseJitter = false,

                ShouldHandle = new PredicateBuilder()
                    .Handle<Exception>(),

                OnRetry = args =>
                {
                    var ex = args.Outcome.Exception;
                    logger.LogError(ex,
                        "Could not send a message due to an exception. Attempt number: {AttemptNumber}. Exception: {Exception}",
                        args.AttemptNumber,
                        ex?.GetBaseException().Message);

                    logger.LogInformation("Retry to send a message");

                    return default;
                }
            });
        });

        return services;
    }
}