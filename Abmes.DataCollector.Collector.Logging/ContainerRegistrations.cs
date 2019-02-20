using Autofac;
using Abmes.DataCollector.Collector.Common.Collecting;
using Abmes.DataCollector.Collector.Common.Destinations;
using Abmes.DataCollector.Collector.Common.Misc;
using Abmes.DataCollector.Collector.Common.Configuration;

namespace Abmes.DataCollector.Collector.Logging
{
    public static class ContainerRegistrations
    {
        public static void RegisterFor(ContainerBuilder builder)
        {
            builder.RegisterType<Collecting.MainCollector>().Named<IMainCollector>("LoggingDecorator");
            builder.RegisterType<Collecting.DataCollector>().Named<IDataCollector>("LoggingDecorator");
            builder.RegisterType<Collecting.DataPreparer>().Named<IDataPreparer>("LoggingDecorator");
            builder.RegisterType<Collecting.DatabPreparePoller>().Named<IDataPreparePoller>("LoggingDecorator");
            builder.RegisterType<Collecting.CollectUrlsProvider>().Named<ICollectUrlsProvider>("LoggingDecorator");
            builder.RegisterType<Collecting.CollectUrlExtractor>().Named<ICollectUrlExtractor>("LoggingDecorator");
            builder.RegisterType<Configuration.ConfigSetNameProvider>().Named<IConfigSetNameProvider>("LoggingDecorator");
            builder.RegisterType<Configuration.DataCollectionsConfigProvider>().Named<IDataCollectionsConfigProvider>("LoggingDecorator");
            builder.RegisterType<Configuration.DestinationsConfigProvider>().Named<IDestinationsConfigProvider>("LoggingDecorator");
            builder.RegisterType<Destinations.Destination>().As<Destinations.ILoggingDestination>();
            builder.RegisterType<Misc.Delay>().Named<IDelay>("LoggingDecorator");
            builder.RegisterType<Destinations.LoggingDestinationResolver>().Named<IDestinationResolver>("LoggingDestinationResolver");

            builder.RegisterDecorator<IMainCollector>((x, inner) => x.ResolveNamed<IMainCollector>("LoggingDecorator", TypedParameter.From(inner)), "base").As<IMainCollector>();
            builder.RegisterDecorator<IDataCollector>((x, inner) => x.ResolveNamed<IDataCollector>("LoggingDecorator", TypedParameter.From(inner)), "base").As<IDataCollector>();
            builder.RegisterDecorator<IDataPreparer>((x, inner) => x.ResolveNamed<IDataPreparer>("LoggingDecorator", TypedParameter.From(inner)), "base").As<IDataPreparer>();
            builder.RegisterDecorator<IDataPreparePoller>((x, inner) => x.ResolveNamed<IDataPreparePoller>("LoggingDecorator", TypedParameter.From(inner)), "base").As<IDataPreparePoller>();
            builder.RegisterDecorator<ICollectUrlsProvider>((x, inner) => x.ResolveNamed<ICollectUrlsProvider>("LoggingDecorator", TypedParameter.From(inner)), "base").As<ICollectUrlsProvider>();
            builder.RegisterDecorator<ICollectUrlExtractor>((x, inner) => x.ResolveNamed<ICollectUrlExtractor>("LoggingDecorator", TypedParameter.From(inner)), "base").As<ICollectUrlExtractor>();
            builder.RegisterDecorator<IConfigSetNameProvider>((x, inner) => x.ResolveNamed<IConfigSetNameProvider>("LoggingDecorator", TypedParameter.From(inner)), "base").Named<IConfigSetNameProvider>("logging");
            builder.RegisterDecorator<IDataCollectionsConfigProvider>((x, inner) => x.ResolveNamed<IDataCollectionsConfigProvider>("LoggingDecorator", TypedParameter.From(inner)), "dateFormatting").As<IDataCollectionsConfigProvider>();
            builder.RegisterDecorator<IDestinationsConfigProvider>((x, inner) => x.ResolveNamed<IDestinationsConfigProvider>("LoggingDecorator", TypedParameter.From(inner)), "base").As<IDestinationsConfigProvider>();
            builder.RegisterDecorator<IDelay>((x, inner) => x.ResolveNamed<IDelay>("LoggingDecorator", TypedParameter.From(inner)), "base").As<IDelay>();
            builder.RegisterDecorator<IDestinationResolver>((x, inner) => x.ResolveNamed<IDestinationResolver>("LoggingDestinationResolver", TypedParameter.From(inner)), "base").As<IDestinationResolver>();
        }
    }
}
