using Abmes.DataCollector.Collector.Data.CommandLine.AppConfig;
using Abmes.DataCollector.Collector.Data.CommandLine.Configuration;
using Abmes.DataCollector.Collector.Services;
using Abmes.DataCollector.Collector.Services.Contracts;
using Abmes.DataCollector.Collector.Services.Ports.AppConfig;
using Abmes.DataCollector.Collector.Services.Ports.Configuration;
using Abmes.DataCollector.Common.Data.Web;
using Abmes.DataCollector.Common.Services.Ports.Configuration;
using Autofac;
using Microsoft.Extensions.Configuration;

namespace Abmes.DataCollector.Collector.App.Library;

public static class ContainerRegistrations
{
    public static void RegisterFor(ContainerBuilder builder, IConfiguration configuration)
    {
        Abmes.DataCollector.Common.Data.Configuration.ContainerRegistrations.RegisterFor(builder);
        Abmes.DataCollector.Common.Data.Amazon.ContainerRegistrations.RegisterFor(builder, configuration);
        Abmes.DataCollector.Common.Data.Azure.ContainerRegistrations.RegisterFor(builder);
        Abmes.DataCollector.Common.Data.FileSystem.ContainerRegistrations.RegisterFor(builder);
        CommonDataWebStartup.ConfigureContainer(builder);
        Abmes.DataCollector.Common.Services.DI.ContainerRegistrations.RegisterFor(builder);

        Abmes.DataCollector.Collector.Data.Amazon.ContainerRegistrations.RegisterFor(builder, configuration);
        Abmes.DataCollector.Collector.Data.Azure.ContainerRegistrations.RegisterFor(builder);
        Abmes.DataCollector.Collector.Data.FileSystem.ContainerRegistrations.RegisterFor(builder);
        Abmes.DataCollector.Collector.Data.Web.ContainerRegistrations.RegisterFor(builder);
        Abmes.DataCollector.Collector.Data.Console.ContainerRegistrations.RegisterFor(builder);

        builder.RegisterType<MainService>().As<IMainService>();
        builder.RegisterType<ConfigSetNameProvider>().Named<IConfigSetNameProvider>("base");
        builder.RegisterType<DataCollectionsFilterProvider>().As<IDataCollectionsFilterProvider>();
        builder.RegisterType<ConfigLocationProvider>().As<IConfigLocationProvider>();
        builder.RegisterType<CollectorModeProvider>().As<ICollectorModeProvider>();
        builder.RegisterType<TimeFilterProvider>().As<ITimeFilterProvider>();

        Services.DI.ContainerRegistrations.RegisterFor(builder);
    }
}
