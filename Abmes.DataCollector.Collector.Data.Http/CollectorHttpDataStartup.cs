using Abmes.DataCollector.Collector.Data.Common.Identity;
using Abmes.DataCollector.Collector.Data.Http.Collecting;
using Abmes.DataCollector.Collector.Data.Http.Collecting.Logging;
using Abmes.DataCollector.Collector.Data.Http.Identity;
using Abmes.DataCollector.Collector.Services.Ports.Collecting;
using Autofac;
using Microsoft.Extensions.DependencyInjection;
using Polly;
using Polly.DependencyInjection;
using Polly.Retry;

namespace Abmes.DataCollector.Collector.Data.Http;

public static class CollectorHttpDataStartup
{
    public static void ConfigureSrvices(IServiceCollection services)
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

    public static void ConfigureContainer(ContainerBuilder builder)
    {
        builder.RegisterType<DataPreparer>().Named<IDataPreparer>("base");
        builder.RegisterType<DataPreparePoller>().Named<IDataPreparePoller>("base");
        builder.RegisterType<CollectItemsProvider>().Named<ICollectItemsProvider>("base");
        builder.RegisterType<CollectUrlExtractor>().Named<ICollectUrlExtractor>("base");

        builder.RegisterType<DataPreparerLoggingDecorator>().Named<IDataPreparer>("LoggingDecorator");
        builder.RegisterDecorator<IDataPreparer>((x, inner) => x.ResolveNamed<IDataPreparer>("LoggingDecorator", TypedParameter.From(inner)), "base").As<IDataPreparer>();

        builder.RegisterType<DataPreparePollerLoggingDecorator>().Named<IDataPreparePoller>("LoggingDecorator");
        builder.RegisterDecorator<IDataPreparePoller>((x, inner) => x.ResolveNamed<IDataPreparePoller>("LoggingDecorator", TypedParameter.From(inner)), "base").As<IDataPreparePoller>();

        builder.RegisterType<CollectItemsProviderLoggingDecorator>().Named<ICollectItemsProvider>("LoggingDecorator");
        builder.RegisterDecorator<ICollectItemsProvider>((x, inner) => x.ResolveNamed<ICollectItemsProvider>("LoggingDecorator", TypedParameter.From(inner)), "base").As<ICollectItemsProvider>();

        builder.RegisterType<CollectUrlExtractorLoggingDecorator>().Named<ICollectUrlExtractor>("LoggingDecorator");
        builder.RegisterDecorator<ICollectUrlExtractor>((x, inner) => x.ResolveNamed<ICollectUrlExtractor>("LoggingDecorator", TypedParameter.From(inner)), "base").As<ICollectUrlExtractor>();

        builder.RegisterType<IdentityServiceHttpRequestConfigurator>().As<IIdentityServiceHttpRequestConfigurator>();
    }
}
