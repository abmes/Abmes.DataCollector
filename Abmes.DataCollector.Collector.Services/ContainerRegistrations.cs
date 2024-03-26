﻿using Autofac;
using Abmes.DataCollector.Collector.Services.Collecting;
using Abmes.DataCollector.Collector.Services.Destinations;
using Abmes.DataCollector.Collector.Services.Misc;
using Abmes.DataCollector.Collector.Services.Configuration;

namespace Abmes.DataCollector.Collector.Services;

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
        builder.RegisterType<DestinationsConfigProvider>().Named<IDestinationsConfigProvider>("base");
        builder.RegisterType<IdentityServiceHttpRequestConfigurator>().As<IIdentityServiceHttpRequestConfigurator>();

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
    }
}