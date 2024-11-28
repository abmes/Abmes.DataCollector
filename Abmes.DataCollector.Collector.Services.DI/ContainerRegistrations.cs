﻿using Abmes.DataCollector.Collector.Services.AppConfig;
using Abmes.DataCollector.Collector.Services.Collecting;
using Abmes.DataCollector.Collector.Services.Configuration;
using Abmes.DataCollector.Collector.Services.Configuration.ConfigSetName.Caching;
using Abmes.DataCollector.Collector.Services.Configuration.ConfigSetName.Logging;
using Abmes.DataCollector.Collector.Services.Destinations;
using Abmes.DataCollector.Collector.Services.Destinations.Configuration;
using Abmes.DataCollector.Collector.Services.Destinations.Configuration.Logging;
using Abmes.DataCollector.Collector.Services.Destinations.Logging;
using Abmes.DataCollector.Collector.Services.Identity;
using Abmes.DataCollector.Collector.Services.Misc;
using Abmes.DataCollector.Collector.Services.Ports.AppConfig;
using Abmes.DataCollector.Collector.Services.Ports.Configuration;
using Abmes.DataCollector.Collector.Services.Ports.Destinations;
using Abmes.DataCollector.Collector.Services.Ports.Identity;
using Autofac;

namespace Abmes.DataCollector.Collector.Services.DI;

public static class ContainerRegistrations
{
    public static void RegisterFor(ContainerBuilder builder)
    {
        builder.RegisterType<DataCollectionsJsonConfigProvider>().As<IDataCollectionsJsonConfigsProvider>();

        builder.RegisterType<MainCollector>().Named<IMainCollector>("base");
        builder.RegisterType<Collecting.DataCollector>().Named<IDataCollector>("base");
        builder.RegisterType<CollectItemsCollector>().Named<ICollectItemsCollector>("base");
        builder.RegisterType<DataPreparer>().Named<IDataPreparer>("base");
        builder.RegisterType<DataPreparePoller>().Named<IDataPreparePoller>("base");
        builder.RegisterType<CollectItemsProvider>().Named<ICollectItemsProvider>("base");
        builder.RegisterType<CollectUrlExtractor>().Named<ICollectUrlExtractor>("base");
        builder.RegisterType<Delay>().Named<IDelay>("base");
        builder.RegisterType<DateTimeFormatter>().As<IDateTimeFormatter>();
        builder.RegisterType<TimeFilterProcessor>().As<ITimeFilterProcessor>();
        builder.RegisterType<DateFormattedDataCollectionConfigProvider>().As<IDateFormattedDataCollectionConfigProvider>();
        builder.RegisterType<MergedDataCollectionConfigProvider>().As<IMergedDataCollectionConfigProvider>();
        builder.RegisterType<DataCollectionsConfigProvider>().Named<IDataCollectionsConfigProvider>("base");

        builder.RegisterType<Bootstrapper>().As<IBootstrapper>().SingleInstance();
        builder.RegisterType<BootstrapperConfigSetNameProvider>().Named<IConfigSetNameProvider>("base");
        builder.RegisterType<BootstrapperDataCollectionsFilterProvider>().As<IDataCollectionsFilterProvider>();
        builder.RegisterType<BootstrapperCollectorModeProvider>().As<ICollectorModeProvider>();
        builder.RegisterType<BootstrapperTimeFilterProvider>().As<ITimeFilterProvider>();

        builder.RegisterType<MergedDataCollectionsConfigProvider>().Named<IDataCollectionsConfigProvider>("MergingDecorator");
        builder.RegisterDecorator<IDataCollectionsConfigProvider>((x, inner) => x.ResolveNamed<IDataCollectionsConfigProvider>("MergingDecorator", TypedParameter.From(inner)), "base").Named<IDataCollectionsConfigProvider>("merging");

        builder.RegisterType<FilteredDataCollectionsConfigProvider>().Named<IDataCollectionsConfigProvider>("FilteringDecorator");
        builder.RegisterDecorator<IDataCollectionsConfigProvider>((x, inner) => x.ResolveNamed<IDataCollectionsConfigProvider>("FilteringDecorator", TypedParameter.From(inner)), "merging").Named<IDataCollectionsConfigProvider>("filtering");

        builder.RegisterType<DateFormattedDataCollectionsConfigProvider>().Named<IDataCollectionsConfigProvider>("DateFormattingDecorator");
        builder.RegisterDecorator<IDataCollectionsConfigProvider>((x, inner) => x.ResolveNamed<IDataCollectionsConfigProvider>("DateFormattingDecorator", TypedParameter.From(inner)), "filtering").Named<IDataCollectionsConfigProvider>("dateFormatting");

        builder.RegisterType<Collecting.Logging.MainCollectorLoggingDecorator>().Named<IMainCollector>("LoggingDecorator");
        builder.RegisterDecorator<IMainCollector>((x, inner) => x.ResolveNamed<IMainCollector>("LoggingDecorator", TypedParameter.From(inner)), "base").As<IMainCollector>();

        builder.RegisterType<Collecting.Logging.DataCollectorLoggingDecorator>().Named<IDataCollector>("LoggingDecorator");
        builder.RegisterDecorator<IDataCollector>((x, inner) => x.ResolveNamed<IDataCollector>("LoggingDecorator", TypedParameter.From(inner)), "base").As<IDataCollector>();

        builder.RegisterType<Collecting.Logging.CollectItemsCollectorLoggingDecorator>().Named<ICollectItemsCollector>("LoggingDecorator");
        builder.RegisterDecorator<ICollectItemsCollector>((x, inner) => x.ResolveNamed<ICollectItemsCollector>("LoggingDecorator", TypedParameter.From(inner)), "base").As<ICollectItemsCollector>();

        builder.RegisterType<Collecting.Logging.DataPreparerLoggingDecorator>().Named<IDataPreparer>("LoggingDecorator");
        builder.RegisterDecorator<IDataPreparer>((x, inner) => x.ResolveNamed<IDataPreparer>("LoggingDecorator", TypedParameter.From(inner)), "base").As<IDataPreparer>();

        builder.RegisterType<Collecting.Logging.DataPreparePollerLoggingDecorator>().Named<IDataPreparePoller>("LoggingDecorator");
        builder.RegisterDecorator<IDataPreparePoller>((x, inner) => x.ResolveNamed<IDataPreparePoller>("LoggingDecorator", TypedParameter.From(inner)), "base").As<IDataPreparePoller>();

        builder.RegisterType<Collecting.Logging.CollectItemsProviderLoggingDecorator>().Named<ICollectItemsProvider>("LoggingDecorator");
        builder.RegisterDecorator<ICollectItemsProvider>((x, inner) => x.ResolveNamed<ICollectItemsProvider>("LoggingDecorator", TypedParameter.From(inner)), "base").As<ICollectItemsProvider>();

        builder.RegisterType<Collecting.Logging.CollectUrlExtractorLoggingDecorator>().Named<ICollectUrlExtractor>("LoggingDecorator");
        builder.RegisterDecorator<ICollectUrlExtractor>((x, inner) => x.ResolveNamed<ICollectUrlExtractor>("LoggingDecorator", TypedParameter.From(inner)), "base").As<ICollectUrlExtractor>();

        builder.RegisterType<Configuration.Logging.DataCollectionsConfigProviderLoggingDecorator>().Named<IDataCollectionsConfigProvider>("LoggingDecorator");
        builder.RegisterDecorator<IDataCollectionsConfigProvider>((x, inner) => x.ResolveNamed<IDataCollectionsConfigProvider>("LoggingDecorator", TypedParameter.From(inner)), "dateFormatting").As<IDataCollectionsConfigProvider>();

        builder.RegisterType<Misc.Logging.DelayLoggingDecorator>().Named<IDelay>("LoggingDecorator");
        builder.RegisterDecorator<IDelay>((x, inner) => x.ResolveNamed<IDelay>("LoggingDecorator", TypedParameter.From(inner)), "base").As<IDelay>();

        builder.RegisterType<DestinationsJsonConfigProvider>().As<IDestinationsJsonConfigProvider>();

        builder.RegisterType<DestinationProvider>().As<IDestinationProvider>();
        builder.RegisterType<DestinationResolverProvider>().As<IDestinationResolverProvider>();

        builder.RegisterType<DestinationsConfigProvider>().Named<IDestinationsConfigProvider>("base");

        builder.RegisterType<DestinationsConfigProviderLoggingDecorator>().Named<IDestinationsConfigProvider>("LoggingDecorator");
        builder.RegisterDecorator<IDestinationsConfigProvider>((x, inner) => x.ResolveNamed<IDestinationsConfigProvider>("LoggingDecorator", TypedParameter.From(inner)), "base").As<IDestinationsConfigProvider>();
        builder.RegisterType<LoggingDestination>().As<ILoggingDestination>();

        builder.RegisterType<LoggingDestinationResolver>().Named<IDestinationResolver>("LoggingDestinationResolver");
        builder.RegisterDecorator<IDestinationResolver>((x, inner) => x.ResolveNamed<IDestinationResolver>("LoggingDestinationResolver", TypedParameter.From(inner)), "base").As<IDestinationResolver>();

        builder.RegisterType<ConfigSetNameProviderCachingDecorator>().Named<IConfigSetNameProvider>("CachingDecorator");
        builder.RegisterDecorator<IConfigSetNameProvider>((x, inner) => x.ResolveNamed<IConfigSetNameProvider>("CachingDecorator", TypedParameter.From(inner)), "logging").As<IConfigSetNameProvider>().SingleInstance();

        builder.RegisterType<ConfigSetNameProviderLoggingDecorator>().Named<IConfigSetNameProvider>("LoggingDecorator");
        builder.RegisterDecorator<IConfigSetNameProvider>((x, inner) => x.ResolveNamed<IConfigSetNameProvider>("LoggingDecorator", TypedParameter.From(inner)), "base").Named<IConfigSetNameProvider>("logging");

        builder.RegisterType<IdentityServiceHttpRequestConfigurator>().As<IIdentityServiceHttpRequestConfigurator>();
    }
}
