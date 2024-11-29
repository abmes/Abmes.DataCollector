using Abmes.DataCollector.Shared.Services.Ports.Configuration;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Abmes.DataCollector.Shared.Data.Configuration;

public static class ServicesConfiguration
{
    public static void Configure(IServiceCollection services, IConfiguration configuration)
    {
        services.Configure<CommonAppSettings>(configuration.GetSection("AppSettings"));
        services.AddOptionsAdapter<ICommonAppSettings, CommonAppSettings>();
    }
}
