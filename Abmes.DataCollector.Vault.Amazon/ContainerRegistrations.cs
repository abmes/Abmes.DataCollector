using Abmes.DataCollector.Collector.Common.Configuration;
using Abmes.DataCollector.Common.Amazon.Configuration;
using Abmes.DataCollector.Vault.Amazon.Configuration;
using Abmes.DataCollector.Vault.Amazon.Storage;
using Abmes.DataCollector.Vault.Configuration;
using Abmes.DataCollector.Vault.Storage;
using Autofac;

namespace Abmes.DataCollector.Vault.Amazon
{
    public static class ContainerRegistrations
    {
        public static void RegisterFor(ContainerBuilder builder)
        {
            builder.RegisterType<AmazonStorage>().Named<IStorage>("baseAmazon");

            builder.RegisterType<ConfigProvider>().Named<IConfigProvider>("base");
            builder.RegisterType<StorageConfigsProvider>().Named<IStorageConfigsProvider>("base");
            builder.RegisterType<UsersProvider>().As<IUsersProvider>();
        }
    }
}
