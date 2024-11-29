using Abmes.DataCollector.Shared.Data.Azure.Configuration;
using Abmes.DataCollector.Shared.Data.Azure.Storage;
using Abmes.DataCollector.Shared.Services.Ports.Configuration;
using Autofac;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Abmes.DataCollector.Shared.Data.Azure;

public static class SharedAzureDataStartup
{
    public static void ConfigureServices(IServiceCollection services, IConfiguration configuration)
    {
        services.Configure<AzureAppSettings>(configuration.GetSection("AppSettings"));
        services.AddOptionsAdapter<IAzureAppSettings, AzureAppSettings>();
    }

    public static void ConfigureContainer(ContainerBuilder builder)
    {
        builder.RegisterType<ConfigLoader>().As<IConfigLoader>();
        builder.RegisterType<AzureCommonStorage>().As<IAzureCommonStorage>();
    }
}
