using Abmes.DataCollector.Vault.Services.Ports.Configuration;

namespace Abmes.DataCollector.Vault.Data.Amazon.Storage;

public delegate IAmazonStorage IAmazonStorageFactory(StorageConfig storageConfig);
