using Abmes.DataCollector.Shared.Services.Configuration;
using Abmes.DataCollector.Shared.Services.Configuration.Caching;
using Abmes.DataCollector.Shared.Services.Configuration.Logging;
using Abmes.DataCollector.Shared.Services.Ports.Configuration;
using Abmes.DataCollector.Shared.Services.Ports.Storage;
using Abmes.DataCollector.Shared.Services.Storage;
using Autofac;

namespace Abmes.DataCollector.Shared.Services.DI;

public static class SharedServicesStartup
{
    public static void ConfigureContainer(ContainerBuilder builder)
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
