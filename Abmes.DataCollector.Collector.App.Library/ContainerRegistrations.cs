using Abmes.DataCollector.Collector.Data.CommandLine.AppConfig;
using Abmes.DataCollector.Collector.Data.CommandLine.Configuration;
using Abmes.DataCollector.Collector.Services;
using Abmes.DataCollector.Collector.Services.Contracts;
using Abmes.DataCollector.Collector.Services.Ports.AppConfig;
using Abmes.DataCollector.Collector.Services.Ports.Configuration;
using Abmes.DataCollector.Shared.Data.Web;
using Abmes.DataCollector.Shared.Services.Ports.Configuration;
using Autofac;
using Microsoft.Extensions.Configuration;

namespace Abmes.DataCollector.Collector.App.Library;

public static class ContainerRegistrations
{
    public static void RegisterFor(ContainerBuilder builder, IConfiguration configuration)
    {
        Abmes.DataCollector.Shared.Data.Configuration.SharedConfigurationDataStartup.ConfigureContainer(builder);
        Abmes.DataCollector.Shared.Data.Amazon.SharedAmazonDataStartup.ConfigureContainer(builder, configuration);
        Abmes.DataCollector.Shared.Data.Azure.SharedAzureDataStartup.ConfigureContainer(builder);
        Abmes.DataCollector.Shared.Data.FileSystem.SharedFileSystemDataStartup.ConfigureContainer(builder);
        SharedWebDataStartup.ConfigureContainer(builder);
        Abmes.DataCollector.Shared.Services.DI.SharedServicesStartup.ConfigureContainer(builder);

        Abmes.DataCollector.Collector.Data.Amazon.CollectorAmazonDataStartup.ConfigureContainer(builder, configuration);
        Abmes.DataCollector.Collector.Data.Azure.CollectorAzureDataStartup.ConfigureContainer(builder);
        Abmes.DataCollector.Collector.Data.FileSystem.CollectorFileSystemDataStartup.ConfigureContainer(builder);
        Abmes.DataCollector.Collector.Data.Web.CollectorWebDataStartup.ConfigureContainer(builder);
        Abmes.DataCollector.Collector.Data.Console.CollectorConsoleDataStartup.ConfigureContainer(builder);
        Abmes.DataCollector.Collector.Data.Http.CollectorHttpDataStartup.ConfigureContainer(builder);

        builder.RegisterType<MainService>().As<IMainService>();
        builder.RegisterType<ConfigSetNameProvider>().Named<IConfigSetNameProvider>("base");
        builder.RegisterType<DataCollectionsFilterProvider>().As<IDataCollectionsFilterProvider>();
        builder.RegisterType<ConfigLocationProvider>().As<IConfigLocationProvider>();
        builder.RegisterType<CollectorModeProvider>().As<ICollectorModeProvider>();
        builder.RegisterType<TimeFilterProvider>().As<ITimeFilterProvider>();

        Services.DI.CollectorServicesStartup.ConfigureContainer(builder);
    }
}
