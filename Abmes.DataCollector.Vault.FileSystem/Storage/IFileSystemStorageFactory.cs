using Abmes.DataCollector.Vault.Configuration;

namespace Abmes.DataCollector.Vault.FileSystem.Storage
{
    public delegate IFileSystemStorage IFileSystemStorageFactory(StorageConfig storageConfig);
}
