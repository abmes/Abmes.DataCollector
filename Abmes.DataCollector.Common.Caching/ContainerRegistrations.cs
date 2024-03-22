using Autofac;
using Abmes.DataCollector.Common.Caching.Cache;
using Abmes.DataCollector.Common.Data.Configuration;

namespace Abmes.DataCollector.Common.Caching;

public static class ContainerRegistrations
{
    public static void RegisterFor(ContainerBuilder builder)
    {
        builder.RegisterType<ConfigFileCache>().As<IConfigFileCache>().SingleInstance();

        builder.RegisterType<Caching.Configuration.ConfigFileProvider>().Named<IConfigProvider>("CachingDecorator");
        builder.RegisterDecorator<IConfigProvider>((x, inner) => x.ResolveNamed<IConfigProvider>("CachingDecorator", TypedParameter.From(inner)), "logging").As<IConfigProvider>();
    }
}
