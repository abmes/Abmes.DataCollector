using Abmes.DataCollector.Vault.Configuration;

namespace Abmes.DataCollector.Vault.Data.Azure.Storage;

public delegate IAzureStorage IAzureStorageFactory(StorageConfig storageConfig);
