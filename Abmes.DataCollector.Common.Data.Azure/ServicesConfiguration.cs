using Abmes.DataCollector.Common.Data.Azure.Configuration;
using Abmes.DataCollector.Utils.DependencyInjection;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Abmes.DataCollector.Common.Data.Azure;

public static class ServicesConfiguration
{
    public static void Configure(IServiceCollection services, IConfiguration configuration)
    {
        services.Configure<AzureAppSettings>(configuration.GetSection("AppSettings"));
        services.AddOptionsAdapter<IAzureAppSettings, AzureAppSettings>();
    }
}
