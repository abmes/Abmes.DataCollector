using Abmes.DataCollector.Vault.Configuration;
using Abmes.DataCollector.Vault.Storage;
using System;
using System.Collections.Generic;
using System.Text;

namespace Abmes.DataCollector.Vault.Azure.Storage
{
    public class AzureStorageResolver : IStorageResolver
    {
        private readonly IAzureStorageFactory _AzureStorageFactory;

        public AzureStorageResolver(
            IAzureStorageFactory AzureStorageFactory)
        {
            _AzureStorageFactory = AzureStorageFactory;
        }

        public bool CanResolve(StorageConfig storageConfig)
        {
            return string.Equals(storageConfig.StorageType, "Azure");
        }

        public IStorage GetStorage(StorageConfig storageConfig)
        {
            return _AzureStorageFactory(storageConfig);
        }
    }
}
