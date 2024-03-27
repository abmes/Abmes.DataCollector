using Abmes.DataCollector.Collector.App.Library.Configuration;
using Abmes.DataCollector.Collector.App.Library.Initialization;
using Abmes.DataCollector.Collector.Common.Configuration;
using Abmes.DataCollector.Collector.Services.Configuration;
using Abmes.DataCollector.Common.Data.Configuration;
using Autofac;
using Microsoft.Extensions.Configuration;

namespace Abmes.DataCollector.Collector.App.Library;

public static class ContainerRegistrations
{
    public static void RegisterFor(ContainerBuilder builder, IConfiguration configuration)
    {
        Abmes.DataCollector.Common.Data.ContainerRegistrations.RegisterFor(builder);
        Abmes.DataCollector.Common.Data.Amazon.ContainerRegistrations.RegisterFor(builder, configuration);
        Abmes.DataCollector.Common.Data.Azure.ContainerRegistrations.RegisterFor(builder);
        Abmes.DataCollector.Common.Data.FileSystem.ContainerRegistrations.RegisterFor(builder);
        Abmes.DataCollector.Common.Data.Web.ContainerRegistrations.RegisterFor(builder);

        Abmes.DataCollector.Collector.Data.ContainerRegistrations.RegisterFor(builder);
        Abmes.DataCollector.Collector.Data.Amazon.ContainerRegistrations.RegisterFor(builder, configuration);
        Abmes.DataCollector.Collector.Data.Azure.ContainerRegistrations.RegisterFor(builder);
        Abmes.DataCollector.Collector.Data.FileSystem.ContainerRegistrations.RegisterFor(builder);
        Abmes.DataCollector.Collector.Data.Web.ContainerRegistrations.RegisterFor(builder);
        Abmes.DataCollector.Collector.Data.Console.ContainerRegistrations.RegisterFor(builder);

        Abmes.DataCollector.Collector.Services.ContainerRegistrations.RegisterFor(builder);

        builder.RegisterType<MainService>().As<IMainService>();
        builder.RegisterType<ConfigSetNameProvider>().Named<IConfigSetNameProvider>("base");
        builder.RegisterType<DataCollectionsFilterProvider>().As<IDataCollectionsFilterProvider>();
        builder.RegisterType<ConfigLocationProvider>().As<IConfigLocationProvider>();
        builder.RegisterType<CollectorModeProvider>().As<ICollectorModeProvider>();
        builder.RegisterType<TimeFilterProvider>().As<ITimeFilterProvider>();

        Abmes.DataCollector.Collector.Common.ContainerRegistrations.RegisterFor(builder);
    }
}
