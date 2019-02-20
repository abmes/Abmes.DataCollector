using Abmes.DataCollector.Collector.Azure.Destinations;
using Abmes.DataCollector.Collector.Common.Destinations;
using Autofac;

namespace Abmes.DataCollector.Collector.Azure
{
    public static class ContainerRegistrations
    {
        public static void RegisterFor(ContainerBuilder builder)
        {
            builder.RegisterType<AzureDestination>().As<IAzureDestination>();
            builder.RegisterType<AzureDestinationResolver>().Named<IDestinationResolver>("base");
        }
    }
}
