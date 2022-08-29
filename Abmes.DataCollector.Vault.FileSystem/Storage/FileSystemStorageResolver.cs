using Abmes.DataCollector.Vault.Configuration;
using Abmes.DataCollector.Vault.Storage;

namespace Abmes.DataCollector.Vault.FileSystem.Storage;

public class FileSystemStorageResolver : IStorageResolver
{
    private readonly IFileSystemStorageFactory _fileSystemStorageFactory;

    public FileSystemStorageResolver(
        IFileSystemStorageFactory fileSystemStorageFactory)
    {
        _fileSystemStorageFactory = fileSystemStorageFactory;
    }

    public bool CanResolve(StorageConfig storageConfig)
    {
        return string.Equals(storageConfig.StorageType, "FileSystem");
    }

    public IStorage GetStorage(StorageConfig storageConfig)
    {
        return _fileSystemStorageFactory(storageConfig);
    }
}
