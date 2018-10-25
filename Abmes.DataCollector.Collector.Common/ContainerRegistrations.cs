using Autofac;
using Microsoft.Extensions.DependencyInjection;
using Abmes.DataCollector.Collector.Common.Collecting;
using Abmes.DataCollector.Common;
using Abmes.DataCollector.Collector.Configuration;
using Abmes.DataCollector.Collector.Common.Destinations;
using Abmes.DataCollector.Collector.Common.Misc;
using System;
using System.Collections.Generic;
using System.Text;

namespace Abmes.DataCollector.Collector.Common
{
    public static class ContainerRegistrations
    {
        public static void RegisterFor(ContainerBuilder builder)
        {
            builder.RegisterType<DestinationsJsonConfigProvider>().As<IDestinationsJsonConfigProvider>();
            builder.RegisterType<DatasCollectJsonConfigProvider>().As<IDataCollectJsonConfigsProvider>();
            builder.RegisterType<DestinationProvider>().As<IDestinationProvider>();
            builder.RegisterType<DestinationFactory>().As<IDestinationFactory>();
            builder.RegisterType<MainCollector>().Named<IMainCollector>("base");
            builder.RegisterType<Collecting.DataCollector>().Named<IDataCollector>("base");
            builder.RegisterType<DataPreparer>().Named<IDataPreparer>("base");
            builder.RegisterType<DataPreparePoller>().Named<IDataPreparePoller>("base");
            builder.RegisterType<CollectUrlsProvider>().As<ICollectUrlsProvider>();
            builder.RegisterType<Delay>().Named<IDelay>("base");
            builder.RegisterType<DateTimeFormatter>().As<IDateTimeFormatter>();
            builder.RegisterType<DateFormattedDataCollectConfigProvider>().As<IDateFormattedDataCollectConfigProvider>();

            builder.RegisterType<FilteredDataCollectConfigsProvider>().Named<IDataCollectConfigsProvider>("FilteringDecorator");
            builder.RegisterDecorator<IDataCollectConfigsProvider>((x, inner) => x.ResolveNamed<IDataCollectConfigsProvider>("FilteringDecorator", TypedParameter.From(inner)), "base").Named<IDataCollectConfigsProvider>("filtering");

            builder.RegisterType<DateFormattedDataCollectConfigsProvider>().Named<IDataCollectConfigsProvider>("DateFormattingDecorator");
            builder.RegisterDecorator<IDataCollectConfigsProvider>((x, inner) => x.ResolveNamed<IDataCollectConfigsProvider>("DateFormattingDecorator", TypedParameter.From(inner)), "filtering").Named<IDataCollectConfigsProvider>("dateFormatting");
        }
    }
}
