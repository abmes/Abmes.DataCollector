using Abmes.DataCollector.Collector.Data.Amazon;
using Abmes.DataCollector.Collector.Data.Azure;
using Abmes.DataCollector.Collector.Data.CommandLine.AppConfig;
using Abmes.DataCollector.Collector.Data.CommandLine.Configuration;
using Abmes.DataCollector.Collector.Data.Console;
using Abmes.DataCollector.Collector.Data.FileSystem;
using Abmes.DataCollector.Collector.Data.Http;
using Abmes.DataCollector.Collector.Data.Web;
using Abmes.DataCollector.Collector.Services;
using Abmes.DataCollector.Collector.Services.Contracts;
using Abmes.DataCollector.Collector.Services.DI;
using Abmes.DataCollector.Collector.Services.Ports.AppConfig;
using Abmes.DataCollector.Collector.Services.Ports.Configuration;
using Abmes.DataCollector.Shared.Data.Amazon;
using Abmes.DataCollector.Shared.Data.Azure;
using Abmes.DataCollector.Shared.Data.Configuration;
using Abmes.DataCollector.Shared.Data.FileSystem;
using Abmes.DataCollector.Shared.Services.DI;
using Abmes.DataCollector.Shared.Services.Ports.Configuration;
using Autofac;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Abmes.DataCollector.Collector.App.Library;

public static class CollectorAppLibraryStartup
{
    public static void ConfigureServices(IServiceCollection services, IConfiguration configuration)
    {
        SharedConfigurationDataStartup.ConfigureServices(services, configuration);
        SharedAmazonDataStartup.ConfigureServices(services, configuration);
        SharedAzureDataStartup.ConfigureServices(services, configuration);
        SharedFileSystemDataStartup.ConfigureServices(services, configuration);

        SharedServicesStartup.ConfigureServices(services);

        CollectorAmazonDataStartup.ConfigureSrvices(services);
        CollectorAzureDataStartup.ConfigureSrvices(services);
        CollectorFileSystemDataStartup.ConfigureSrvices(services);
        CollectorWebDataStartup.ConfigureServices(services, configuration);
        CollectorConsoleDataStartup.ConfigureSrvices(services);
        CollectorHttpDataStartup.ConfigureSrvices(services);

        CollectorServicesStartup.ConfigureServices(services);
    }

    public static void ConfigureContainer(ContainerBuilder builder, IConfiguration configuration)
    {
        SharedConfigurationDataStartup.ConfigureContainer(builder);
        SharedAmazonDataStartup.ConfigureContainer(builder, configuration);
        SharedAzureDataStartup.ConfigureContainer(builder);
        SharedFileSystemDataStartup.ConfigureContainer(builder);

        SharedServicesStartup.ConfigureContainer(builder);

        CollectorAmazonDataStartup.ConfigureContainer(builder, configuration);
        CollectorAzureDataStartup.ConfigureContainer(builder);
        CollectorFileSystemDataStartup.ConfigureContainer(builder);
        CollectorWebDataStartup.ConfigureContainer(builder);
        CollectorConsoleDataStartup.ConfigureContainer(builder);
        CollectorHttpDataStartup.ConfigureContainer(builder);

        builder.RegisterType<MainService>().As<IMainService>();
        builder.RegisterType<ConfigSetNameProvider>().Named<IConfigSetNameProvider>("base");
        builder.RegisterType<DataCollectionsFilterProvider>().As<IDataCollectionsFilterProvider>();
        builder.RegisterType<ConfigLocationProvider>().As<IConfigLocationProvider>();
        builder.RegisterType<CollectorModeProvider>().As<ICollectorModeProvider>();
        builder.RegisterType<TimeFilterProvider>().As<ITimeFilterProvider>();

        CollectorServicesStartup.ConfigureContainer(builder);
    }
}
