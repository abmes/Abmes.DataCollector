using Abmes.DataCollector.Utils.DependencyInjection;
using Abmes.DataCollector.Vault.Configuration;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Abmes.DataCollector.Vault;

public static class ServicesConfiguration
{
    public static void Configure(IServiceCollection services, IConfiguration configuration)
    {
        services.Configure<VaultAppSettings>(configuration.GetSection("AppSettings"));
        services.AddOptionsAdapter<IVaultAppSettings, VaultAppSettings>();
    }
}
