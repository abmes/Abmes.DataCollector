using Abmes.DataCollector.Collector.Data.Console.Destinations;
using Abmes.DataCollector.Collector.Services.Ports.Destinations;
using Autofac;

namespace Abmes.DataCollector.Collector.Data.Console;

public static class CollectorConsoleDataStartup
{
    public static void ConfigureContainer(ContainerBuilder builder)
    {
        builder.RegisterType<ConsoleDestination>().As<IConsoleDestination>();
        builder.RegisterType<ConsoleDestinationResolver>().Named<IDestinationResolver>("base");
    }
}
