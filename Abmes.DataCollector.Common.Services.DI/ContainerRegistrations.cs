using Abmes.DataCollector.Common.Services.Configuration;
using Abmes.DataCollector.Common.Services.Configuration.Caching;
using Abmes.DataCollector.Common.Services.Configuration.Logging;
using Abmes.DataCollector.Common.Services.Ports.Configuration;
using Abmes.DataCollector.Common.Services.Ports.Storage;
using Abmes.DataCollector.Common.Services.Storage;
using Autofac;

namespace Abmes.DataCollector.Common.Services.DI;

public static class ContainerRegistrations
{
    public static void RegisterFor(ContainerBuilder builder)
    {
        builder.RegisterType<FileNameProvider>().As<IFileNameProvider>();

        builder.RegisterType<ConfigProvider>().Named<IConfigProvider>("base");

        builder.RegisterType<ConfigFileCache>().As<IConfigFileCache>().SingleInstance();

        builder.RegisterType<ConfigFileProviderCachingDecorator>().Named<IConfigProvider>("CachingDecorator");
        builder.RegisterDecorator<IConfigProvider>((x, inner) => x.ResolveNamed<IConfigProvider>("CachingDecorator", TypedParameter.From(inner)), "logging").As<IConfigProvider>();

        builder.RegisterType<ConfigProviderLoggingDecorator>().Named<IConfigProvider>("LoggingDecorator");
        builder.RegisterDecorator<IConfigProvider>((x, inner) => x.ResolveNamed<IConfigProvider>("LoggingDecorator", TypedParameter.From(inner)), "base").Named<IConfigProvider>("logging");
    }
}
