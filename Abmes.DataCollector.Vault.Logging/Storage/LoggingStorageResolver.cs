using Abmes.DataCollector.Vault.Configuration;
using Abmes.DataCollector.Vault.Storage;

namespace Abmes.DataCollector.Vault.Logging.Storage;

public class LoggingStorageResolver : IStorageResolver
{
    private readonly IStorageResolver _storageResolver;
    private readonly ILoggingStorageFactory _loggingStorageFactory;

    public LoggingStorageResolver(
        IStorageResolver StorageResolver,
        ILoggingStorageFactory loggingStorageFactory)
    {
        _storageResolver = StorageResolver;
        _loggingStorageFactory = loggingStorageFactory;
    }

    public bool CanResolve(IStorageConfig storageConfig)
    {
        return _storageResolver.CanResolve(storageConfig);
    }

    public IStorage GetStorage(IStorageConfig storageConfig)
    {
        var storage = _storageResolver.GetStorage(storageConfig);
        return _loggingStorageFactory(storage);
    }
}
