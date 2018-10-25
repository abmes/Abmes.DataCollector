using Autofac;
using Abmes.DataCollector.Vault.Configuration;
using Abmes.DataCollector.Vault.Services;
using Abmes.DataCollector.Vault.Storage;

namespace Abmes.DataCollector.Vault
{
    public static class ContainerRegistrations
    {
        public static void RegisterFor(ContainerBuilder builder)
        {
            builder.RegisterType<UsersJsonProvider>().As<IUsersJsonProvider>();
            builder.RegisterType<StorageJsonConfigsProvider>().As<IStorageJsonConfigsProvider>();
            builder.RegisterType<StoragesProvider>().As<IStoragesProvider>();
            builder.RegisterType<StorageFactory>().As<IStorageFactory>();
            builder.RegisterType<DataCollectionFiles>().Named<IDataCollectionFiles>("base");
        }
    }
}
