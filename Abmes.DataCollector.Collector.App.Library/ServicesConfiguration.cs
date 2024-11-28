using Abmes.DataCollector.Common.Data.Web;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Abmes.DataCollector.Collector.App.Library;

public static class ServicesConfiguration
{
    public static void Configure(IServiceCollection services, IConfiguration configuration)
    {
        Abmes.DataCollector.Common.Data.Configuration.ServicesConfiguration.Configure(services, configuration);
        Abmes.DataCollector.Common.Data.Amazon.ServicesConfiguration.Configure(services, configuration);
        Abmes.DataCollector.Common.Data.Azure.ServicesConfiguration.Configure(services, configuration);
        Abmes.DataCollector.Common.Data.FileSystem.ServicesConfiguration.Configure(services, configuration);
        CommonDataWebStartup.ConfigureServices(services, configuration);

        Services.DI.ServicesConfiguration.Configure(services);
    }
}
