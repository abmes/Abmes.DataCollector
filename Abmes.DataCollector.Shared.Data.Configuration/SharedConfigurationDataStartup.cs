using Abmes.DataCollector.Shared.Services.Ports.Configuration;
using Autofac;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Abmes.DataCollector.Shared.Data.Configuration;

public static class SharedConfigurationDataStartup
{
    public static void ConfigureServices(IServiceCollection services, IConfiguration configuration)
    {
        services.Configure<CommonAppSettings>(configuration.GetSection("AppSettings"));
        services.AddOptionsAdapter<ICommonAppSettings, CommonAppSettings>();
    }

    public static void ConfigureContainer(ContainerBuilder builder)
    {
    }
}
