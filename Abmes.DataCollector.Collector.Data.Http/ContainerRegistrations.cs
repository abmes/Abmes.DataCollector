using Abmes.DataCollector.Collector.Data.Common.Identity;
using Abmes.DataCollector.Collector.Data.Http.Collecting;
using Abmes.DataCollector.Collector.Data.Http.Collecting.Logging;
using Abmes.DataCollector.Collector.Data.Http.Identity;
using Abmes.DataCollector.Collector.Services.Ports.Collecting;
using Autofac;

namespace Abmes.DataCollector.Collector.Data.Http;

public static class ContainerRegistrations
{
    public static void RegisterFor(ContainerBuilder builder)
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
