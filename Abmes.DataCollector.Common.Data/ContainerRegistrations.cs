using Abmes.DataCollector.Common.Data.Configuration;
using Abmes.DataCollector.Common.Data.Storage;
using Autofac;

namespace Abmes.DataCollector.Common.Data;

public static class ContainerRegistrations
{
    public static void RegisterFor(ContainerBuilder builder)
    {
        builder.RegisterType<ConfigProvider>().Named<IConfigProvider>("base");
        builder.RegisterType<FileNameProvider>().As<IFileNameProvider>();

        builder.RegisterType<Configuration.Caching.ConfigFileCache>().As<Configuration.Caching.IConfigFileCache>().SingleInstance();

        builder.RegisterType<Configuration.Caching.ConfigFileProvider>().Named<IConfigProvider>("CachingDecorator");
        builder.RegisterDecorator<IConfigProvider>((x, inner) => x.ResolveNamed<IConfigProvider>("CachingDecorator", TypedParameter.From(inner)), "logging").As<IConfigProvider>();

        builder.RegisterType<Configuration.Logging.ConfigProvider>().Named<IConfigProvider>("LoggingDecorator");
        builder.RegisterDecorator<IConfigProvider>((x, inner) => x.ResolveNamed<IConfigProvider>("LoggingDecorator", TypedParameter.From(inner)), "base").Named<IConfigProvider>("logging");
    }
}
