using Abmes.DataCollector.Collector.Amazon.Destinations;
using Abmes.DataCollector.Collector.Common.Destinations;
using Autofac;

namespace Abmes.DataCollector.Collector.Amazon
{
    public static class ContainerRegistrations
    {
        public static void RegisterFor(ContainerBuilder builder)
        {
            builder.RegisterType<AmazonDestination>().Named<IDestination>("baseAmazon");
        }
    }
}
