using Autofac;
using Abmes.DataCollector.Vault.Configuration;
using Abmes.DataCollector.Vault.Services;
using Abmes.DataCollector.Vault.Storage;
using Abmes.DataCollector.Collector.Common.Configuration;

namespace Abmes.DataCollector.Vault.Logging
{
    public static class ContainerRegistrations
    {
        public static void RegisterFor(ContainerBuilder builder)
        {
            builder.RegisterType<Logging.Configuration.ConfigProvider>().Named<IConfigProvider>("LoggingDecorator");
            builder.RegisterType<Logging.Configuration.StorageConfigProvider>().Named<IStoragesConfigProvider>("LoggingDecorator");
            builder.RegisterType<Logging.Configuration.DataCollectionNameProvider>().Named<IDataCollectionNameProvider>("LoggingDecorator");
            builder.RegisterType<Logging.Storage.Storage>().Named<IStorage>("LoggingDecorator");
            builder.RegisterType<Logging.Services.DataCollectionFiles>().Named<IDataCollectionFiles>("LoggingDecorator");

            builder.RegisterDecorator<IConfigProvider>((x, inner) => x.ResolveNamed<IConfigProvider>("LoggingDecorator", TypedParameter.From(inner)), "baseAmazon").As<IConfigProvider>();
            builder.RegisterDecorator<IStoragesConfigProvider>((x, inner) => x.ResolveNamed<IStoragesConfigProvider>("LoggingDecorator", TypedParameter.From(inner)), "base").As<IStoragesConfigProvider>();
            builder.RegisterDecorator<IDataCollectionNameProvider>((x, inner) => x.ResolveNamed<IDataCollectionNameProvider>("LoggingDecorator", TypedParameter.From(inner)), "base").As<IDataCollectionNameProvider>();
            builder.RegisterDecorator<IStorage>((x, inner) => x.ResolveNamed<IStorage>("LoggingDecorator", TypedParameter.From(inner)), "baseAmazon").Keyed<IStorage>("Amazon");
            builder.RegisterDecorator<IStorage>((x, inner) => x.ResolveNamed<IStorage>("LoggingDecorator", TypedParameter.From(inner)), "baseAzure").Keyed<IStorage>("Azure");
            builder.RegisterDecorator<IDataCollectionFiles>((x, inner) => x.ResolveNamed<IDataCollectionFiles>("LoggingDecorator", TypedParameter.From(inner)), "base").As<IDataCollectionFiles>();
        }
    }
}
