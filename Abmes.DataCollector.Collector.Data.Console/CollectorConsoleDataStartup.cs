using Abmes.DataCollector.Collector.Data.Console.Destinations;
using Abmes.DataCollector.Collector.Services.Ports.Destinations;
using Autofac;
using Microsoft.Extensions.DependencyInjection;

namespace Abmes.DataCollector.Collector.Data.Console;

public static class CollectorConsoleDataStartup
{
    public static void ConfigureSrvices(IServiceCollection services)
    {
    }

    public static void ConfigureContainer(ContainerBuilder builder)
    {
        builder.RegisterType<ConsoleDestination>().As<IConsoleDestination>();
        builder.RegisterType<ConsoleDestinationResolver>().Named<IDestinationResolver>("base");
    }
}
