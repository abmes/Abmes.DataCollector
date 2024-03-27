using Abmes.DataCollector.Collector.Data.Amazon.Collecting;
using Abmes.DataCollector.Collector.Data.Amazon.Destinations;
using Abmes.DataCollector.Collector.Data.Collecting;
using Abmes.DataCollector.Collector.Data.Destinations;
using Autofac;
using Microsoft.Extensions.Configuration;

namespace Abmes.DataCollector.Collector.Data.Amazon;

public static class ContainerRegistrations
{
    public static void RegisterFor(ContainerBuilder builder, IConfiguration configuration)
    {
        if (Abmes.DataCollector.Common.Data.Amazon.ContainerRegistrations.AmazonRegistrationNeeded(configuration))
        {
            builder.RegisterType<AmazonDestination>().As<IAmazonDestination>();
            builder.RegisterType<AmazonDestinationResolver>().Named<IDestinationResolver>("base");
        }

        builder.RegisterType<AmazonSimpleContentProvider>().As<ISimpleContentProvider>();
    }
}
