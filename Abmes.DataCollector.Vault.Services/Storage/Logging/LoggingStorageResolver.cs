using Abmes.DataCollector.Vault.Services.Ports.Configuration;
using Abmes.DataCollector.Vault.Services.Ports.Storage;

namespace Abmes.DataCollector.Vault.Services.Storage.Logging;

public class LoggingStorageResolver(
    IStorageResolver storageResolver,
    ILoggingStorageFactory loggingStorageFactory)
    : IStorageResolver
{
    public bool CanResolve(StorageConfig storageConfig)
    {
        return storageResolver.CanResolve(storageConfig);
    }

    public IStorage GetStorage(StorageConfig storageConfig)
    {
        var storage = storageResolver.GetStorage(storageConfig);
        return loggingStorageFactory(storage);
    }
}
