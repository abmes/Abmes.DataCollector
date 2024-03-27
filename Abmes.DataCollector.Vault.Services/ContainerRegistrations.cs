using Autofac;

namespace Abmes.DataCollector.Vault.Services;

public static class ContainerRegistrations
{
    public static void RegisterFor(ContainerBuilder builder)
    {
        builder.RegisterType<DataCollectionFiles>().Named<IDataCollectionFiles>("base");
    }
}
