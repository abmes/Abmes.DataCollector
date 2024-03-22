using Abmes.DataCollector.Collector.Common.Collecting;
using Abmes.DataCollector.Utils.Polly;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Polly;
using Polly.DependencyInjection;
using Polly.Retry;

namespace Abmes.DataCollector.Collector.Common;

public static class ServicesConfiguration
{
    public static void Configure(IServiceCollection services, IConfiguration configuration)
    {
        services.AddPollyAsyncExecutionStrategy<CollectItemsCollector.ICollectToDestinationMarker>(CollectItemsCollectorCollectToDestinationRetryingStrategyConfig);
        services.AddPollyAsyncExecutionStrategy<CollectItemsCollector.ICollectItemsMarker>(ParallelOperationRetryingStrategyConfig);
        services.AddPollyAsyncExecutionStrategy<CollectItemsCollector.IGarbageCollectTargetsMarker>(ParallelOperationRetryingStrategyConfig);

        services.AddPollyAsyncExecutionStrategy<CollectItemsProvider.IGetCollectItemsMarker>(CollectItemsProviderGetCollectItemsRetryingStrategyConfig);
        services.AddPollyAsyncExecutionStrategy< CollectItemsProvider.IRedirectCollectItemsMarker> (ParallelOperationRetryingStrategyConfig);

        services.AddPollyAsyncExecutionStrategy<CollectUrlExtractor>(CollectUrlExtractorRetryingStrategyConfig);
        
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

    private static void CollectItemsProviderGetCollectItemsRetryingStrategyConfig(ResiliencePipelineBuilder builder, AddResiliencePipelineContext<Type> context)
    {
        builder
            .AddRetry(new RetryStrategyOptions
            {
                ShouldHandle = new PredicateBuilder().Handle<Exception>(),
                BackoffType = DelayBackoffType.Constant,
                Delay = TimeSpan.FromSeconds(5),  // todo: config
                MaxRetryAttempts = 1  // todo: config
            });
    }

    private static void CollectUrlExtractorRetryingStrategyConfig(ResiliencePipelineBuilder builder, AddResiliencePipelineContext<Type> context)
    {
        builder
            .AddRetry(new RetryStrategyOptions
            {
                ShouldHandle = new PredicateBuilder().Handle<Exception>(),
                BackoffType = DelayBackoffType.Constant,
                Delay = TimeSpan.FromSeconds(10),  // todo: config
                MaxRetryAttempts = 5  // todo: config
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
