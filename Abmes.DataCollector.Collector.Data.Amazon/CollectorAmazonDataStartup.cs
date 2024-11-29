using Abmes.DataCollector.Collector.Data.Amazon.Collecting;
using Abmes.DataCollector.Collector.Data.Amazon.Destinations;
using Abmes.DataCollector.Collector.Services.Ports.Collecting;
using Abmes.DataCollector.Collector.Services.Ports.Destinations;
using Autofac;
using Microsoft.Extensions.Configuration;

namespace Abmes.DataCollector.Collector.Data.Amazon;

public static class CollectorAmazonDataStartup
{
    public static void ConfigureContainer(ContainerBuilder builder, IConfiguration configuration)
    {
        if (Abmes.DataCollector.Shared.Data.Amazon.SharedAmazonDataStartup.IsAmazonRegistrationNeeded(configuration))
        {
            builder.RegisterType<AmazonDestination>().As<IAmazonDestination>();
            builder.RegisterType<AmazonDestinationResolver>().Named<IDestinationResolver>("base");
        }

        builder.RegisterType<AmazonSimpleContentProvider>().As<ISimpleContentProvider>();
    }
}
