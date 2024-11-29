using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Abmes.DataCollector.Collector.Web.Library;

public static class ServicesConfiguration
{
    public static void Configure(IServiceCollection services, IConfiguration configuration)
    {
        Abmes.DataCollector.Shared.Data.Configuration.ServicesConfiguration.Configure(services, configuration);
        Abmes.DataCollector.Shared.Data.Amazon.ServicesConfiguration.Configure(services, configuration);
        Abmes.DataCollector.Shared.Data.Azure.ServicesConfiguration.Configure(services, configuration);

        Abmes.DataCollector.Collector.Data.Http.ServicesConfiguration.Configure(services);

        Services.DI.ServicesConfiguration.Configure(services);
    }
}
