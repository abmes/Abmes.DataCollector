using Abmes.DataCollector.Vault.Configuration;

namespace Abmes.DataCollector.Vault.Azure.Storage;

public delegate IAzureStorage IAzureStorageFactory(IStorageConfig storageConfig);
