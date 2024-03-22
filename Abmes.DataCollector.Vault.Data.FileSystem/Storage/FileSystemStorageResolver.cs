using Abmes.DataCollector.Vault.Configuration;
using Abmes.DataCollector.Vault.Storage;

namespace Abmes.DataCollector.Vault.Data.FileSystem.Storage;

public class FileSystemStorageResolver(
    IFileSystemStorageFactory fileSystemStorageFactory) : IStorageResolver
{
    public bool CanResolve(StorageConfig storageConfig)
    {
        return string.Equals(storageConfig.StorageType, "FileSystem");
    }

    public IStorage GetStorage(StorageConfig storageConfig)
    {
        return fileSystemStorageFactory(storageConfig);
    }
}
