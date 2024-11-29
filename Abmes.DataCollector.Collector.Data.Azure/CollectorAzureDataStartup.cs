using Abmes.DataCollector.Collector.Data.Azure.Destinations;
using Abmes.DataCollector.Collector.Services.Ports.Destinations;
using Autofac;
using Microsoft.Extensions.DependencyInjection;

namespace Abmes.DataCollector.Collector.Data.Azure;

public static class CollectorAzureDataStartup
{
    public static void ConfigureSrvices(IServiceCollection services)
    {
    }

    public static void ConfigureContainer(ContainerBuilder builder)
    {
        builder.RegisterType<AzureDestination>().As<IAzureDestination>();
        builder.RegisterType<AzureDestinationResolver>().Named<IDestinationResolver>("base");
    }
}
