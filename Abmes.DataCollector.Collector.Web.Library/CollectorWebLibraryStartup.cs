using Abmes.DataCollector.Collector.Data.Amazon;
using Abmes.DataCollector.Collector.Data.Azure;
using Abmes.DataCollector.Collector.Data.Console;
using Abmes.DataCollector.Collector.Data.Http;
using Abmes.DataCollector.Collector.Data.Web;
using Abmes.DataCollector.Collector.Services.DI;
using Abmes.DataCollector.Shared.Data.Amazon;
using Abmes.DataCollector.Shared.Data.Azure;
using Abmes.DataCollector.Shared.Data.Configuration;
using Abmes.DataCollector.Shared.Services.DI;
using Autofac;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Abmes.DataCollector.Collector.Web.Library;

public static class CollectorWebLibraryStartup
{
    public static void ConfigureServices(IServiceCollection services, IConfiguration configuration)
    {
        SharedConfigurationDataStartup.ConfigureServices(services, configuration);
        SharedAmazonDataStartup.ConfigureServices(services, configuration);
        SharedAzureDataStartup.ConfigureServices(services, configuration);

        SharedServicesStartup.ConfigureServices(services);

        CollectorAmazonDataStartup.ConfigureSrvices(services);
        CollectorAzureDataStartup.ConfigureSrvices(services);
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

        SharedServicesStartup.ConfigureContainer(builder);

        CollectorAmazonDataStartup.ConfigureContainer(builder, configuration);
        CollectorAzureDataStartup.ConfigureContainer(builder);
        CollectorWebDataStartup.ConfigureContainer(builder);
        CollectorConsoleDataStartup.ConfigureContainer(builder);
        CollectorHttpDataStartup.ConfigureContainer(builder);

        CollectorServicesStartup.ConfigureContainer(builder);
    }
}
