using Autofac;

namespace Abmes.DataCollector.Vault.Services;

public static class ContainerRegistrations
{
    public static void RegisterFor(ContainerBuilder builder)
    {
        builder.RegisterType<DataCollectionFiles>().Named<IDataCollectionFiles>("base");

        builder.RegisterType<Logging.DataCollectionFilesLoggingDecorator>().Named<IDataCollectionFiles>("LoggingDecorator");
        builder.RegisterDecorator<IDataCollectionFiles>((x, inner) => x.ResolveNamed<IDataCollectionFiles>("LoggingDecorator", TypedParameter.From(inner)), "base").As<IDataCollectionFiles>();
    }
}
