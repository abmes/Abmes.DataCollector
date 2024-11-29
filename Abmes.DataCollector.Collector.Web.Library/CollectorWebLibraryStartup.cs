using Autofac;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Abmes.DataCollector.Collector.Web.Library;

public static class CollectorWebLibraryStartup
{
    public static void ConfigureServices(IServiceCollection services, IConfiguration configuration)
    {
        Abmes.DataCollector.Shared.Data.Configuration.SharedConfigurationDataStartup.ConfigureServices(services, configuration);
        Abmes.DataCollector.Shared.Data.Amazon.SharedAmazonDataStartup.ConfigureServices(services, configuration);
        Abmes.DataCollector.Shared.Data.Azure.SharedAzureDataStartup.ConfigureServices(services, configuration);

        Abmes.DataCollector.Shared.Services.DI.SharedServicesStartup.ConfigureServices(services);

        Abmes.DataCollector.Collector.Data.Http.CollectorHttpDataStartup.ConfigureSrvices(services);

        Services.DI.CollectorServicesStartup.ConfigureServices(services);
    }

    public static void ConfigureContainer(ContainerBuilder builder, IConfiguration configuration)
    {
        Abmes.DataCollector.Shared.Data.Configuration.SharedConfigurationDataStartup.ConfigureContainer(builder);
        Abmes.DataCollector.Shared.Data.Amazon.SharedAmazonDataStartup.ConfigureContainer(builder, configuration);
        Abmes.DataCollector.Shared.Data.Azure.SharedAzureDataStartup.ConfigureContainer(builder);

        Abmes.DataCollector.Shared.Services.DI.SharedServicesStartup.ConfigureContainer(builder);

        Abmes.DataCollector.Collector.Data.Amazon.CollectorAmazonDataStartup.ConfigureContainer(builder, configuration);
        Abmes.DataCollector.Collector.Data.Azure.CollectorAzureDataStartup.ConfigureContainer(builder);
        Abmes.DataCollector.Collector.Data.Web.CollectorWebDataStartup.ConfigureContainer(builder);
        Abmes.DataCollector.Collector.Data.Console.CollectorConsoleDataStartup.ConfigureContainer(builder);
        Abmes.DataCollector.Collector.Data.Http.CollectorHttpDataStartup.ConfigureContainer(builder);

        Services.DI.CollectorServicesStartup.ConfigureContainer(builder);
    }
}
