using Abmes.DataCollector.Collector.Data.Console.Destinations;
using Abmes.DataCollector.Collector.Services.Destinations;
using Autofac;

namespace Abmes.DataCollector.Collector.Data.Console;

public static class ContainerRegistrations
{
    public static void RegisterFor(ContainerBuilder builder)
    {
        builder.RegisterType<ConsoleDestination>().As<IConsoleDestination>();
        builder.RegisterType<ConsoleDestinationResolver>().Named<IDestinationResolver>("base");
    }
}
