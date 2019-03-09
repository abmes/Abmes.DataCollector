using Autofac;
using Abmes.DataCollector.Collector.Common.Collecting;
using Abmes.DataCollector.Collector.Common.Destinations;
using Abmes.DataCollector.Collector.Common.Misc;
using Abmes.DataCollector.Collector.Common.Configuration;

namespace Abmes.DataCollector.Collector.Common
{
    public static class ContainerRegistrations
    {
        public static void RegisterFor(ContainerBuilder builder)
        {
            builder.RegisterType<DestinationsJsonConfigProvider>().As<IDestinationsJsonConfigProvider>();
            builder.RegisterType<DataCollectionsJsonConfigProvider>().As<IDataCollectionsJsonConfigsProvider>();
            builder.RegisterType<DestinationProvider>().As<IDestinationProvider>();
            builder.RegisterType<DestinationResolverProvider>().As<IDestinationResolverProvider>();
            builder.RegisterType<MainCollector>().Named<IMainCollector>("base");
            builder.RegisterType<Collecting.DataCollector>().Named<IDataCollector>("base");
            builder.RegisterType<DataPreparer>().Named<IDataPreparer>("base");
            builder.RegisterType<DataPreparePoller>().Named<IDataPreparePoller>("base");
            builder.RegisterType<CollectUrlsProvider>().Named<ICollectUrlsProvider>("base");
            builder.RegisterType<CollectUrlExtractor>().Named<ICollectUrlExtractor>("base");
            builder.RegisterType<Delay>().Named<IDelay>("base");
            builder.RegisterType<DateTimeFormatter>().As<IDateTimeFormatter>();
            builder.RegisterType<DateFormattedDataCollectionConfigProvider>().As<IDateFormattedDataCollectionConfigProvider>();
            builder.RegisterType<DataCollectionsConfigProvider>().Named<IDataCollectionsConfigProvider>("base");
            builder.RegisterType<DestinationsConfigProvider>().Named<IDestinationsConfigProvider>("base");
            builder.RegisterType<IdentityServiceClientInfo>().As<IIdentityServiceClientInfo>();
            builder.RegisterType<IdentityServiceHttpRequestConfigurator>().As<IIdentityServiceHttpRequestConfigurator>();

            builder.RegisterType<Bootstrapper>().As<IBootstrapper>().SingleInstance();
            builder.RegisterType<BootstrapperConfigSetNameProvider>().As<IConfigSetNameProvider>();
            builder.RegisterType<BootstrapperDataCollectionsFilterProvider>().As<IDataCollectionsFilterProvider>();
            
            builder.RegisterType<FilteredDataCollectionsConfigProvider>().Named<IDataCollectionsConfigProvider>("FilteringDecorator");
            builder.RegisterDecorator<IDataCollectionsConfigProvider>((x, inner) => x.ResolveNamed<IDataCollectionsConfigProvider>("FilteringDecorator", TypedParameter.From(inner)), "base").Named<IDataCollectionsConfigProvider>("filtering");

            builder.RegisterType<DateFormattedDataCollectionsConfigProvider>().Named<IDataCollectionsConfigProvider>("DateFormattingDecorator");
            builder.RegisterDecorator<IDataCollectionsConfigProvider>((x, inner) => x.ResolveNamed<IDataCollectionsConfigProvider>("DateFormattingDecorator", TypedParameter.From(inner)), "filtering").Named<IDataCollectionsConfigProvider>("dateFormatting");
        }
    }
}
