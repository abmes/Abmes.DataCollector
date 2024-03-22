using Abmes.DataCollector.Vault.Data.Amazon.Storage;
using Abmes.DataCollector.Vault.Storage;
using Autofac;

namespace Abmes.DataCollector.Vault.Data.Amazon;

public static class ContainerRegistrations
{
    public static void RegisterFor(ContainerBuilder builder)
    {
        builder.RegisterType<AmazonStorage>().As<IAmazonStorage>();
        builder.RegisterType<AmazonStorageResolver>().Named<IStorageResolver>("base");
    }
}
