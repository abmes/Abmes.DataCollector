using Abmes.DataCollector.Shared.Data.Web;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Abmes.DataCollector.Collector.App.Library;

public static class ServicesConfiguration
{
    public static void Configure(IServiceCollection services, IConfiguration configuration)
    {
        Abmes.DataCollector.Shared.Data.Configuration.SharedConfigurationDataStartup.ConfigureServices(services, configuration);
        Abmes.DataCollector.Shared.Data.Amazon.SharedAmazonDataStartup.ConfigureServices(services, configuration);
        Abmes.DataCollector.Shared.Data.Azure.SharedAzureDataStartup.ConfigureServices(services, configuration);
        Abmes.DataCollector.Shared.Data.FileSystem.SharedFileSystemDataStartup.ConfigureServices(services, configuration);
        SharedWebDataStartup.ConfigureServices(services, configuration);

        Abmes.DataCollector.Collector.Data.Http.CollectorHttpDataStartup.ConfigureSrvices(services);

        Services.DI.CollectorServicesStartup.ConfigureServices(services);
    }
}
