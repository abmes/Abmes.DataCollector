using Abmes.DataCollector.Shared.Services.Ports.Configuration;
using Abmes.DataCollector.Vault.Data.Empty.Configuration;
using Autofac;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Abmes.DataCollector.Vault.Data.Empty;

public static class VaultEmptyDataStartup
{
    public static void ConfigureServices(IServiceCollection services, IConfiguration configuration)
    {
    }

    public static void ConfigureContainer(ContainerBuilder builder)
    {
        builder.RegisterType<EmptyConfigLocationProvider>().As<IConfigLocationProvider>();
    }
}
