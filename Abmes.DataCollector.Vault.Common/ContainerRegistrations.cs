using Autofac;
using Abmes.DataCollector.Vault.Configuration;
using Abmes.DataCollector.Vault.Services;
using Abmes.DataCollector.Vault.Storage;
using Abmes.DataCollector.Vault.Common.Configuration;

namespace Abmes.DataCollector.Vault
{
    public static class ContainerRegistrations
    {
        public static void RegisterFor(ContainerBuilder builder)
        {
            builder.RegisterType<StorageConfigsProvider>().Named<IStorageConfigsProvider>("base");
            builder.RegisterType<UsersProvider>().As<IUsersProvider>();
            builder.RegisterType<UsersJsonProvider>().As<IUsersJsonProvider>();
            builder.RegisterType<StorageJsonConfigsProvider>().As<IStorageJsonConfigsProvider>();
            builder.RegisterType<StoragesProvider>().As<IStoragesProvider>();
            builder.RegisterType<StorageFactory>().As<IStorageFactory>();
            builder.RegisterType<DataCollectionFiles>().Named<IDataCollectionFiles>("base");
        }
    }
}
