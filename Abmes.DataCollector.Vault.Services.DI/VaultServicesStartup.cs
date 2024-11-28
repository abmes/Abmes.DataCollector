using Abmes.DataCollector.Vault.Services.Collecting;
using Abmes.DataCollector.Vault.Services.Collecting.Logging;
using Abmes.DataCollector.Vault.Services.Configuration;
using Abmes.DataCollector.Vault.Services.Contracts;
using Abmes.DataCollector.Vault.Services.Ports.Configuration;
using Abmes.DataCollector.Vault.Services.Ports.Storage;
using Abmes.DataCollector.Vault.Services.Storage;
using Abmes.DataCollector.Vault.Services.Users;
using Autofac;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Abmes.DataCollector.Vault.Services.DI;

public static class VaultServicesStartup
{
    public static void ConfigureServices(IServiceCollection services, IConfiguration configuration)
    {
    }

    public static void ConfigureContainer(ContainerBuilder builder)
    {
        builder.RegisterType<DataCollectionFilesService>().Named<IDataCollectionFilesService>("base");

        builder.RegisterType<DataCollectionFilesServiceLoggingDecorator>().Named<IDataCollectionFilesService>("LoggingDecorator");
        builder.RegisterDecorator<IDataCollectionFilesService>((x, inner) => x.ResolveNamed<IDataCollectionFilesService>("LoggingDecorator", TypedParameter.From(inner)), "base").As<IDataCollectionFilesService>();

        builder.RegisterType<UsersProvider>().As<IUsersProvider>();
        builder.RegisterType<UsersJsonProvider>().As<IUsersJsonProvider>();

        builder.RegisterType<Configuration.Logging.DataCollectionNameProviderLoggingDecorator>().Named<IDataCollectionNameProvider>("LoggingDecorator");
        builder.RegisterDecorator<IDataCollectionNameProvider>((x, inner) => x.ResolveNamed<IDataCollectionNameProvider>("LoggingDecorator", TypedParameter.From(inner)), "base").As<IDataCollectionNameProvider>();

        builder.RegisterType<StoragesProvider>().As<IStoragesProvider>();
        builder.RegisterType<StorageResolverProvider>().As<IStorageResolverProvider>();
        builder.RegisterType<StorageProvider>().As<IStorageProvider>();

        builder.RegisterType<Storage.Logging.LoggingStorageResolver>().Named<IStorageResolver>("LoggingStorageResolver");
        builder.RegisterDecorator<IStorageResolver>((x, inner) => x.ResolveNamed<IStorageResolver>("LoggingStorageResolver", TypedParameter.From(inner)), "base").As<IStorageResolver>();

        builder.RegisterType<Storage.Logging.LoggingStorage>().As<Storage.Logging.ILoggingStorage>();

        builder.RegisterType<StoragesConfigProvider>().Named<IStoragesConfigProvider>("base");
        builder.RegisterType<StoragesJsonConfigProvider>().As<IStoragesJsonConfigProvider>();

        builder.RegisterType<Configuration.Logging.StoragesConfigProviderLoggingDecorator>().Named<IStoragesConfigProvider>("LoggingDecorator");
        builder.RegisterDecorator<IStoragesConfigProvider>((x, inner) => x.ResolveNamed<IStoragesConfigProvider>("LoggingDecorator", TypedParameter.From(inner)), "base").As<IStoragesConfigProvider>();

        builder.RegisterType<UserService>().As<IUserService>();
    }
}
