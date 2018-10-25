using Abmes.DataCollector.Collector.Amazon.Configuration;
using Abmes.DataCollector.Collector.Amazon.Destinations;
using Abmes.DataCollector.Collector.Configuration;
using Abmes.DataCollector.Collector.Common.Destinations;
using Autofac;

namespace Abmes.DataCollector.Collector.Amazon
{
    public static class ContainerRegistrations
    {
        public static void RegisterFor(ContainerBuilder builder)
        {
            builder.RegisterType<AmazonDestination>().Named<IDestination>("baseAmazon");

            builder.RegisterType<DataCollectConfigsProvider>().Named<IDataCollectConfigsProvider>("base");
            builder.RegisterType<DestinationsConfigProvider>().Named<IDestinationsConfigProvider>("base");
        }
    }
}
