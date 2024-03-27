using Abmes.DataCollector.Common.Data.Configuration;
using Abmes.DataCollector.Vault.Data.Configuration;
using Abmes.DataCollector.Vault.Data.Storage;
using Autofac;

namespace Abmes.DataCollector.Vault.Data;

public static class ContainerRegistrations
{
    public static void RegisterFor(ContainerBuilder builder)
    {
        builder.RegisterType<StoragesConfigProvider>().Named<IStoragesConfigProvider>("base");
        builder.RegisterType<UsersProvider>().As<IUsersProvider>();
        builder.RegisterType<UsersJsonProvider>().As<IUsersJsonProvider>();
        builder.RegisterType<StoragesJsonConfigProvider>().As<IStoragesJsonConfigProvider>();
        builder.RegisterType<StoragesProvider>().As<IStoragesProvider>();
        builder.RegisterType<StorageResolverProvider>().As<IStorageResolverProvider>();
        builder.RegisterType<StorageProvider>().As<IStorageProvider>();
        builder.RegisterType<EmptyConfigLocationProvider>().As<IConfigLocationProvider>();
    }
}
