using Abmes.DataCollector.Collector.Console.Destinations;
using Abmes.DataCollector.Collector.Common.Destinations;
using Autofac;

namespace Abmes.DataCollector.Collector.Console;

public static class ContainerRegistrations
{
    public static void RegisterFor(ContainerBuilder builder)
    {
        builder.RegisterType<ConsoleDestination>().As<IConsoleDestination>();
        builder.RegisterType<ConsoleDestinationResolver>().Named<IDestinationResolver>("base");
    }
}
