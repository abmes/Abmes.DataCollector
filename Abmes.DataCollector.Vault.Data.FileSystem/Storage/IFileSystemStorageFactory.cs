using Abmes.DataCollector.Vault.Configuration;

namespace Abmes.DataCollector.Vault.Data.FileSystem.Storage;

public delegate IFileSystemStorage IFileSystemStorageFactory(StorageConfig storageConfig);
