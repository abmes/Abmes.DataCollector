using Abmes.DataCollector.Collector.Amazon.Destinations;
using Abmes.DataCollector.Collector.Common.Destinations;
using Autofac;
using Microsoft.Extensions.Configuration;

namespace Abmes.DataCollector.Collector.Amazon;

public static class ContainerRegistrations
{
    public static void RegisterFor(ContainerBuilder builder, IConfiguration configuration)
    {
        if (Abmes.DataCollector.Common.Amazon.ContainerRegistrations.AmazonRegistrationNeeded(configuration))
        {
            builder.RegisterType<AmazonDestination>().As<IAmazonDestination>();
            builder.RegisterType<AmazonDestinationResolver>().Named<IDestinationResolver>("base");
        }
    }
}
