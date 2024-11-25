using Abmes.DataCollector.Vault.Services.Ports.Configuration;

namespace Abmes.DataCollector.Vault.Data.Azure.Storage;

public delegate IAzureStorage IAzureStorageFactory(StorageConfig storageConfig);
