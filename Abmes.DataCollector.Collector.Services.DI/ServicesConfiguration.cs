using Abmes.DataCollector.Collector.Services.Collecting;
using Abmes.DataCollector.Utils.Polly;
using Microsoft.Extensions.DependencyInjection;
using Polly;
using Polly.DependencyInjection;
using Polly.Retry;

namespace Abmes.DataCollector.Collector.Services.DI;

public static class ServicesConfiguration
{
    public static void Configure(IServiceCollection services)
    {
        services.AddSingleton(TimeProvider.System);

        services.AddPollyAsyncExecutionStrategy<CollectItemsCollector.ICollectToDestinationMarker>(CollectItemsCollectorCollectToDestinationRetryingStrategyConfig);
        services.AddPollyAsyncExecutionStrategy<CollectItemsCollector.ICollectItemsMarker>(ParallelOperationRetryingStrategyConfig);
        services.AddPollyAsyncExecutionStrategy<CollectItemsCollector.IGarbageCollectTargetsMarker>(ParallelOperationRetryingStrategyConfig);

        services.AddPollyAsyncExecutionStrategy<Collecting.DataCollector>(ParallelOperationRetryingStrategyConfig);
    }

    private static void CollectItemsCollectorCollectToDestinationRetryingStrategyConfig(ResiliencePipelineBuilder builder, AddResiliencePipelineContext<Type> context)
    {
        builder
            .AddRetry(new RetryStrategyOptions
            {
                ShouldHandle = new PredicateBuilder().Handle<Exception>(CollectItemsCollector.IsRetryableCollectError),
                BackoffType = DelayBackoffType.Constant,
                Delay = TimeSpan.FromSeconds(5),  // todo: config
                MaxRetryAttempts = 2  // todo: config
            });
    }

    private static void ParallelOperationRetryingStrategyConfig(ResiliencePipelineBuilder builder, AddResiliencePipelineContext<Type> context)
    {
        builder
            .AddRetry(new RetryStrategyOptions
            {
                ShouldHandle = new PredicateBuilder().Handle<Exception>(),
                BackoffType = DelayBackoffType.Linear,
                Delay = TimeSpan.FromSeconds(30),  // todo: config
                MaxRetryAttempts = 2  // todo: config
            });
    }
}
