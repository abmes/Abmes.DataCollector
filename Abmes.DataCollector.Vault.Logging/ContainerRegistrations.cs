using Autofac;
using Abmes.DataCollector.Vault.Configuration;
using Abmes.DataCollector.Vault.Services;
using Abmes.DataCollector.Vault.Storage;
using Abmes.DataCollector.Common.Data.Configuration;

namespace Abmes.DataCollector.Vault.Logging;

public static class ContainerRegistrations
{
    public static void RegisterFor(ContainerBuilder builder)
    {
        builder.RegisterType<Logging.Configuration.StorageConfigProvider>().Named<IStoragesConfigProvider>("LoggingDecorator");
        builder.RegisterType<Logging.Configuration.DataCollectionNameProvider>().Named<IDataCollectionNameProvider>("LoggingDecorator");
        builder.RegisterType<Logging.Storage.Storage>().As<Logging.Storage.ILoggingStorage>();
        builder.RegisterType<Logging.Services.DataCollectionFiles>().Named<IDataCollectionFiles>("LoggingDecorator");
        builder.RegisterType<Logging.Storage.LoggingStorageResolver>().Named<IStorageResolver>("LoggingStorageResolver");

        builder.RegisterDecorator<IConfigProvider>((x, inner) => x.ResolveNamed<IConfigProvider>("LoggingDecorator", TypedParameter.From(inner)), "base").As<IConfigProvider>();
        builder.RegisterDecorator<IStoragesConfigProvider>((x, inner) => x.ResolveNamed<IStoragesConfigProvider>("LoggingDecorator", TypedParameter.From(inner)), "base").As<IStoragesConfigProvider>();
        builder.RegisterDecorator<IDataCollectionNameProvider>((x, inner) => x.ResolveNamed<IDataCollectionNameProvider>("LoggingDecorator", TypedParameter.From(inner)), "base").As<IDataCollectionNameProvider>();
        builder.RegisterDecorator<IDataCollectionFiles>((x, inner) => x.ResolveNamed<IDataCollectionFiles>("LoggingDecorator", TypedParameter.From(inner)), "base").As<IDataCollectionFiles>();
        builder.RegisterDecorator<IStorageResolver>((x, inner) => x.ResolveNamed<IStorageResolver>("LoggingStorageResolver", TypedParameter.From(inner)), "base").As<IStorageResolver>();
    }
}
