using Abmes.DataCollector.Collector.Common.Configuration;
using Autofac;

namespace Abmes.DataCollector.Collector.Caching;

public static class ContainerRegistrations
{
    public static void RegisterFor(ContainerBuilder builder)
    {
        builder.RegisterType<Caching.Configuration.ConfigSetNameProvider>().Named<IConfigSetNameProvider>("CachingDecorator");
        builder.RegisterDecorator<IConfigSetNameProvider>((x, inner) => x.ResolveNamed<IConfigSetNameProvider>("CachingDecorator", TypedParameter.From(inner)), "logging").As<IConfigSetNameProvider>().SingleInstance();
    }
}
