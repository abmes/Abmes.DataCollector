using Abmes.DataCollector.Vault.Data.Azure.Storage;
using Abmes.DataCollector.Vault.Storage;
using Autofac;

namespace Abmes.DataCollector.Vault.Data.Azure;

public static class ContainerRegistrations
{
    public static void RegisterFor(ContainerBuilder builder)
    {
        builder.RegisterType<AzureStorage>().As<IAzureStorage>();
        builder.RegisterType<AzureStorageResolver>().Named<IStorageResolver>("base");
    }
}
