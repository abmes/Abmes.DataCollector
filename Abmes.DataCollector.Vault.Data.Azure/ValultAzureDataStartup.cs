using Abmes.DataCollector.Vault.Data.Azure.Storage;
using Abmes.DataCollector.Vault.Services.Ports.Storage;
using Autofac;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Abmes.DataCollector.Vault.Data.Azure;

public static class ValultAzureDataStartup
{
    public static void ConfigureServices(IServiceCollection services, IConfiguration configuration)
    {
    }

    public static void ConfigureContainer(ContainerBuilder builder)
    {
        builder.RegisterType<AzureStorage>().As<IAzureStorage>();
        builder.RegisterType<AzureStorageResolver>().Named<IStorageResolver>("base");
    }
}
