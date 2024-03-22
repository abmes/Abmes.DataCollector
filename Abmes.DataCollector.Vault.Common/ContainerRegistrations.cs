using Autofac;
using Abmes.DataCollector.Vault.Configuration;
using Abmes.DataCollector.Vault.Services;
using Abmes.DataCollector.Vault.Storage;
using Abmes.DataCollector.Vault.Common.Configuration;
using Abmes.DataCollector.Common.Data.Configuration;

namespace Abmes.DataCollector.Vault;

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
        builder.RegisterType<DataCollectionFiles>().Named<IDataCollectionFiles>("base");
        builder.RegisterType<EmptyConfigLocationProvider>().As<IConfigLocationProvider>();
    }
}
