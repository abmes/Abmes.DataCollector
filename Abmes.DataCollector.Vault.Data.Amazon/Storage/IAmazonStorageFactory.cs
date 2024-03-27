using Abmes.DataCollector.Vault.Data.Configuration;

namespace Abmes.DataCollector.Vault.Data.Amazon.Storage;

public delegate IAmazonStorage IAmazonStorageFactory(StorageConfig storageConfig);
