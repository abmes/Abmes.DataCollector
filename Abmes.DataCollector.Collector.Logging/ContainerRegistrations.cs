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
            builder.RegisterType<Configuration.ConfigProvider>().Named<IConfigProvider>("LoggingDecorator");
            builder.RegisterType<Configuration.ConfigSetNameProvider>().Named<IConfigSetNameProvider>("LoggingDecorator");
            builder.RegisterType<Configuration.DataCollectionsConfigProvider>().Named<IDataCollectionsConfigProvider>("LoggingDecorator");
            builder.RegisterType<Configuration.DestinationsConfigProvider>().Named<IDestinationsConfigProvider>("LoggingDecorator");
            builder.RegisterType<Destinations.Destination>().Named<IDestination>("LoggingDecorator");
            builder.RegisterType<Misc.Delay>().Named<IDelay>("LoggingDecorator");

            builder.RegisterDecorator<IMainCollector>((x, inner) => x.ResolveNamed<IMainCollector>("LoggingDecorator", TypedParameter.From(inner)), "base").Named<IMainCollector>("logging");
            builder.RegisterDecorator<IDataCollector>((x, inner) => x.ResolveNamed<IDataCollector>("LoggingDecorator", TypedParameter.From(inner)), "base").As<IDataCollector>();
            builder.RegisterDecorator<IDataPreparer>((x, inner) => x.ResolveNamed<IDataPreparer>("LoggingDecorator", TypedParameter.From(inner)), "base").As<IDataPreparer>();
            builder.RegisterDecorator<IDataPreparePoller>((x, inner) => x.ResolveNamed<IDataPreparePoller>("LoggingDecorator", TypedParameter.From(inner)), "base").As<IDataPreparePoller>();
            builder.RegisterDecorator<IConfigProvider>((x, inner) => x.ResolveNamed<IConfigProvider>("LoggingDecorator", TypedParameter.From(inner)), "base").Named<IConfigProvider>("logging");
            builder.RegisterDecorator<IConfigSetNameProvider>((x, inner) => x.ResolveNamed<IConfigSetNameProvider>("LoggingDecorator", TypedParameter.From(inner)), "base").As<IConfigSetNameProvider>();
            builder.RegisterDecorator<IDataCollectionsConfigProvider>((x, inner) => x.ResolveNamed<IDataCollectionsConfigProvider>("LoggingDecorator", TypedParameter.From(inner)), "dateFormatting").As<IDataCollectionsConfigProvider>();
            builder.RegisterDecorator<IDestinationsConfigProvider>((x, inner) => x.ResolveNamed<IDestinationsConfigProvider>("LoggingDecorator", TypedParameter.From(inner)), "base").As<IDestinationsConfigProvider>();
            builder.RegisterDecorator<IDelay>((x, inner) => x.ResolveNamed<IDelay>("LoggingDecorator", TypedParameter.From(inner)), "base").As<IDelay>();

            builder.RegisterDecorator<IDestination>((x, inner) => x.ResolveNamed<IDestination>("LoggingDecorator", TypedParameter.From(inner)), "baseAmazon").Keyed<IDestination>("Amazon");
        }
    }
}
