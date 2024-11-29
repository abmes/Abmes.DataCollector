using Abmes.DataCollector.Shared.Data.Azure.Configuration;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Abmes.DataCollector.Shared.Data.Azure;

public static class ServicesConfiguration
{
    public static void Configure(IServiceCollection services, IConfiguration configuration)
    {
        services.Configure<AzureAppSettings>(configuration.GetSection("AppSettings"));
        services.AddOptionsAdapter<IAzureAppSettings, AzureAppSettings>();
    }
}
