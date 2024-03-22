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
        services.AddPollyAsyncExecutionStrategy<CollectItemsCollector>(CollectItemsCollectorRetryingStrategyConfig);
        services.AddPollyAsyncExecutionStrategy<CollectItemsProvider>(CollectItemsProviderRetryingStrategyConfig);
        services.AddPollyAsyncExecutionStrategy<CollectUrlExtractor>(CollectUrlExtractorRetryingStrategyConfig);
    }

    private static void CollectItemsCollectorRetryingStrategyConfig(ResiliencePipelineBuilder builder, AddResiliencePipelineContext<Type> context)
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

    private static void CollectItemsProviderRetryingStrategyConfig(ResiliencePipelineBuilder builder, AddResiliencePipelineContext<Type> context)
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
}
