using Abmes.DataCollector.Vault.Services.Ports.Configuration;
using Abmes.DataCollector.Vault.Services.Ports.Users;
using Autofac;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Abmes.DataCollector.Vault.Data.AspNetCore;

public static class VaultAspNetCoreDataStartup
{
    public static void ConfigureServices(IServiceCollection services, IConfiguration configuration)
    {
        services.AddHttpContextAccessor();
    }

    public static void ConfigureContainer(ContainerBuilder builder)
    {
        builder.RegisterType<DataCollectionNameProvider>().Named<IDataCollectionNameProvider>("base");
        builder.RegisterType<UserExternalIdentifierProvider>().As<IUserExternalIdentifierProvider>();
    }
}
