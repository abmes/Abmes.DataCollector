using Abmes.DataCollector.Collector.Common.Configuration;
using Autofac;

namespace Abmes.DataCollector.Collector.Common;

public static class ContainerRegistrations
{
    public static void RegisterFor(ContainerBuilder builder)
    {
        builder.RegisterType<Configuration.Caching.CachingConfigSetNameProvider>().Named<IConfigSetNameProvider>("CachingDecorator");
        builder.RegisterDecorator<IConfigSetNameProvider>((x, inner) => x.ResolveNamed<IConfigSetNameProvider>("CachingDecorator", TypedParameter.From(inner)), "logging").As<IConfigSetNameProvider>().SingleInstance();

        builder.RegisterType<Configuration.Logging.LoggingConfigSetNameProvider>().Named<IConfigSetNameProvider>("LoggingDecorator");
        builder.RegisterDecorator<IConfigSetNameProvider>((x, inner) => x.ResolveNamed<IConfigSetNameProvider>("LoggingDecorator", TypedParameter.From(inner)), "base").Named<IConfigSetNameProvider>("logging");
    }
}
