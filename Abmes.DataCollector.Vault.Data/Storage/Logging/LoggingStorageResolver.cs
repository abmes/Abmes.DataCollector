using Abmes.DataCollector.Vault.Data.Configuration;

namespace Abmes.DataCollector.Vault.Data.Storage.Logging;

public class LoggingStorageResolver(
    IStorageResolver storageResolver,
    ILoggingStorageFactory loggingStorageFactory) : IStorageResolver
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
