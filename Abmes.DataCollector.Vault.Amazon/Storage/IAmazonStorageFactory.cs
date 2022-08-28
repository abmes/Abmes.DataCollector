using Abmes.DataCollector.Vault.Configuration;

namespace Abmes.DataCollector.Vault.Amazon.Storage;

public delegate IAmazonStorage IAmazonStorageFactory(IStorageConfig storageConfig);
