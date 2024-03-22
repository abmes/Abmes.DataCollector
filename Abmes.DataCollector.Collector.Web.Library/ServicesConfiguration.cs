using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Abmes.DataCollector.Collector.Web.Library;

public static class ServicesConfiguration
{
    public static void Configure(IServiceCollection services, IConfiguration configuration)
    {
        Abmes.DataCollector.Common.Data.ServicesConfiguration.Configure(services, configuration);
        Abmes.DataCollector.Common.Data.Amazon.ServicesConfiguration.Configure(services, configuration);
        Abmes.DataCollector.Common.Data.Azure.ServicesConfiguration.Configure(services, configuration);

        Abmes.DataCollector.Collector.Common.ServicesConfiguration.Configure(services, configuration);
    }
}
