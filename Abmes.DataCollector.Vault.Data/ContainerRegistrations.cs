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

        builder.RegisterType<Configuration.Logging.StoragesConfigProviderLoggingDecorator>().Named<IStoragesConfigProvider>("LoggingDecorator");
        builder.RegisterDecorator<IStoragesConfigProvider>((x, inner) => x.ResolveNamed<IStoragesConfigProvider>("LoggingDecorator", TypedParameter.From(inner)), "base").As<IStoragesConfigProvider>();

        builder.RegisterType<Configuration.Logging.DataCollectionNameProviderLoggingDecorator>().Named<IDataCollectionNameProvider>("LoggingDecorator");
        builder.RegisterDecorator<IDataCollectionNameProvider>((x, inner) => x.ResolveNamed<IDataCollectionNameProvider>("LoggingDecorator", TypedParameter.From(inner)), "base").As<IDataCollectionNameProvider>();

        builder.RegisterType<Storage.Logging.LoggingStorageResolver>().Named<IStorageResolver>("LoggingStorageResolver");
        builder.RegisterDecorator<IStorageResolver>((x, inner) => x.ResolveNamed<IStorageResolver>("LoggingStorageResolver", TypedParameter.From(inner)), "base").As<IStorageResolver>();

        builder.RegisterType<Storage.Logging.LoggingStorage>().As<Storage.Logging.ILoggingStorage>();
    }
}
