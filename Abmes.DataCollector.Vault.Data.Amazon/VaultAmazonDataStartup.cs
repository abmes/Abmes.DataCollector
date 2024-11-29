using Abmes.DataCollector.Vault.Data.Amazon.Storage;
using Abmes.DataCollector.Vault.Services.Ports.Storage;
using Autofac;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Abmes.DataCollector.Vault.Data.Amazon;

public static class VaultAmazonDataStartup
{
    public static void ConfigureServices(IServiceCollection services, IConfiguration configuration)
    {
    }

    public static void ConfigureContainer(ContainerBuilder builder)
    {
        builder.RegisterType<AmazonStorage>().As<IAmazonStorage>();
        builder.RegisterType<AmazonStorageResolver>().Named<IStorageResolver>("base");
    }
}
