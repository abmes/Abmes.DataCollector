using Abmes.DataCollector.Vault.Services.Ports.Configuration;
using Abmes.DataCollector.Vault.Services.Ports.Users;
using Autofac;

namespace Abmes.DataCollector.Vault.Data.AspNetCore;

public static class ContainerRegistrations
{
    public static void RegisterFor(ContainerBuilder builder)
    {

        builder.RegisterType<DataCollectionNameProvider>().Named<IDataCollectionNameProvider>("base");
        builder.RegisterType<UserExternalIdentifierProvider>().As<IUserExternalIdentifierProvider>();
    }
}
