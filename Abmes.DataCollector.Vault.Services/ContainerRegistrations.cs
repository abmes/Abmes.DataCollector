using Abmes.DataCollector.Vault.Services.Collecting;
using Abmes.DataCollector.Vault.Services.Collecting.Logging;
using Abmes.DataCollector.Vault.Services.Configuration;
using Abmes.DataCollector.Vault.Services.Contracts;
using Autofac;

namespace Abmes.DataCollector.Vault.Services;

public static class ContainerRegistrations
{
    public static void RegisterFor(ContainerBuilder builder)
    {
        builder.RegisterType<DataCollectionFiles>().Named<IDataCollectionFiles>("base");

        builder.RegisterType<DataCollectionFilesLoggingDecorator>().Named<IDataCollectionFiles>("LoggingDecorator");
        builder.RegisterDecorator<IDataCollectionFiles>((x, inner) => x.ResolveNamed<IDataCollectionFiles>("LoggingDecorator", TypedParameter.From(inner)), "base").As<IDataCollectionFiles>();

        builder.RegisterType<UsersProvider>().As<IUsersProvider>();
        builder.RegisterType<UsersJsonProvider>().As<IUsersJsonProvider>();

        builder.RegisterType<Configuration.Logging.DataCollectionNameProviderLoggingDecorator>().Named<IDataCollectionNameProvider>("LoggingDecorator");
        builder.RegisterDecorator<IDataCollectionNameProvider>((x, inner) => x.ResolveNamed<IDataCollectionNameProvider>("LoggingDecorator", TypedParameter.From(inner)), "base").As<IDataCollectionNameProvider>();
    }
}
