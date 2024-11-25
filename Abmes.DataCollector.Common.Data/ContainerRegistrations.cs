using Abmes.DataCollector.Common.Data.Configuration;
using Autofac;

namespace Abmes.DataCollector.Common.Data;

public static class ContainerRegistrations
{
    public static void RegisterFor(ContainerBuilder builder)
    {
        builder.RegisterType<ConfigProvider>().Named<IConfigProvider>("base");

        builder.RegisterType<Configuration.Caching.ConfigFileCache>().As<Configuration.Caching.IConfigFileCache>().SingleInstance();

        builder.RegisterType<Configuration.Caching.ConfigFileProviderCachingDecorator>().Named<IConfigProvider>("CachingDecorator");
        builder.RegisterDecorator<IConfigProvider>((x, inner) => x.ResolveNamed<IConfigProvider>("CachingDecorator", TypedParameter.From(inner)), "logging").As<IConfigProvider>();

        builder.RegisterType<Configuration.Logging.ConfigProviderLoggingDecorator>().Named<IConfigProvider>("LoggingDecorator");
        builder.RegisterDecorator<IConfigProvider>((x, inner) => x.ResolveNamed<IConfigProvider>("LoggingDecorator", TypedParameter.From(inner)), "base").Named<IConfigProvider>("logging");
    }
}
