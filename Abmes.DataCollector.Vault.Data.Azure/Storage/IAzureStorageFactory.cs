using Abmes.DataCollector.Vault.Data.Configuration;

namespace Abmes.DataCollector.Vault.Data.Azure.Storage;

public delegate IAzureStorage IAzureStorageFactory(StorageConfig storageConfig);
