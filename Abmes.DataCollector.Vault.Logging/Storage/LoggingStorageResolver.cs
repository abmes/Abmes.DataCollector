using Abmes.DataCollector.Vault.Configuration;
using Abmes.DataCollector.Vault.Storage;

namespace Abmes.DataCollector.Vault.Logging.Storage;

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
