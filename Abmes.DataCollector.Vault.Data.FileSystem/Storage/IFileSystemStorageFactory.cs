using Abmes.DataCollector.Vault.Data.Configuration;

namespace Abmes.DataCollector.Vault.Data.FileSystem.Storage;

public delegate IFileSystemStorage IFileSystemStorageFactory(StorageConfig storageConfig);
