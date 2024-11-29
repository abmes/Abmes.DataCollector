using Abmes.DataCollector.Collector.Data.CommandLine.AppConfig;
using Abmes.DataCollector.Collector.Data.CommandLine.Configuration;
using Abmes.DataCollector.Collector.Services.Ports.AppConfig;
using Abmes.DataCollector.Collector.Services.Ports.Configuration;
using Abmes.DataCollector.Shared.Services.Ports.Configuration;
using Autofac;
using Microsoft.Extensions.DependencyInjection;

namespace Abmes.DataCollector.Collector.Data.CommandLine;

public static class CollectorCommandLineDataStartup
{
    public static void ConfigureSrvices(IServiceCollection services)
    {
    }

    public static void ConfigureContainer(ContainerBuilder builder)
    {
        builder.RegisterType<ConfigSetNameProvider>().Named<IConfigSetNameProvider>("base");
        builder.RegisterType<DataCollectionsFilterProvider>().As<IDataCollectionsFilterProvider>();
        builder.RegisterType<ConfigLocationProvider>().As<IConfigLocationProvider>();
        builder.RegisterType<CollectorModeProvider>().As<ICollectorModeProvider>();
        builder.RegisterType<TimeFilterProvider>().As<ITimeFilterProvider>();

    }
}
