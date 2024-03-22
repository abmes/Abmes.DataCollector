using Abmes.DataCollector.Vault.Configuration;

namespace Abmes.DataCollector.Vault.Data.Amazon.Storage;

public delegate IAmazonStorage IAmazonStorageFactory(StorageConfig storageConfig);
