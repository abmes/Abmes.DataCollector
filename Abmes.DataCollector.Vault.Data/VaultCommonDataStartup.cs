using Abmes.DataCollector.Vault.Data.Configuration;
using Autofac;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Abmes.DataCollector.Vault.Data;

public static class VaultCommonDataStartup
{
    public static void ConfigureServices(IServiceCollection services, IConfiguration configuration)
    {
        services.Configure<VaultAppSettings>(configuration.GetSection("AppSettings"));
        services.AddOptionsAdapter<IVaultAppSettings, VaultAppSettings>();
    }

    public static void ConfigureContainer(ContainerBuilder builder)
    {
    }
}
