using Abmes.DataCollector.Vault.Services.Ports.Configuration;

namespace Abmes.DataCollector.Vault.Data.FileSystem.Storage;

public delegate IFileSystemStorage IFileSystemStorageFactory(StorageConfig storageConfig);
