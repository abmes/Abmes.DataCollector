using Abmes.DataCollector.Collector.Data.Configuration;
using Abmes.DataCollector.Collector.Data.Destinations;
using Autofac;

namespace Abmes.DataCollector.Collector.Data;

public static class ContainerRegistrations
{
    public static void RegisterFor(ContainerBuilder builder)
    {
        builder.RegisterType<DestinationsJsonConfigProvider>().As<IDestinationsJsonConfigProvider>();

        builder.RegisterType<DestinationProvider>().As<IDestinationProvider>();
        builder.RegisterType<DestinationResolverProvider>().As<IDestinationResolverProvider>();

        builder.RegisterType<DestinationsConfigProvider>().Named<IDestinationsConfigProvider>("base");

        builder.RegisterType<Configuration.Logging.DestinationsConfigProviderLoggingDecorator>().Named<IDestinationsConfigProvider>("LoggingDecorator");
        builder.RegisterDecorator<IDestinationsConfigProvider>((x, inner) => x.ResolveNamed<IDestinationsConfigProvider>("LoggingDecorator", TypedParameter.From(inner)), "base").As<IDestinationsConfigProvider>();

        builder.RegisterType<Destinations.Logging.LoggingDestination>().As<Destinations.Logging.ILoggingDestination>();

        builder.RegisterType<Destinations.Logging.LoggingDestinationResolver>().Named<IDestinationResolver>("LoggingDestinationResolver");
        builder.RegisterDecorator<IDestinationResolver>((x, inner) => x.ResolveNamed<IDestinationResolver>("LoggingDestinationResolver", TypedParameter.From(inner)), "base").As<IDestinationResolver>();
    }
}
