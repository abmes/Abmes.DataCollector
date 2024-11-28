using Abmes.DataCollector.Utils.Polly;
using Microsoft.Extensions.DependencyInjection;
using Polly.DependencyInjection;
using Polly.Retry;
using Polly;
using Abmes.DataCollector.Collector.Data.Http.Collecting;

namespace Abmes.DataCollector.Collector.Data.Http;

public static class ServicesConfiguration
{
    public static void Configure(IServiceCollection services)
    {
        services.AddPollyAsyncExecutionStrategy<CollectItemsProvider.IGetCollectItemsMarker>(CollectItemsProviderGetCollectItemsRetryingStrategyConfig);
        services.AddPollyAsyncExecutionStrategy<CollectItemsProvider.IRedirectCollectItemsMarker>(ParallelOperationRetryingStrategyConfig);

        services.AddPollyAsyncExecutionStrategy<CollectUrlExtractor>(CollectUrlExtractorRetryingStrategyConfig);
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
