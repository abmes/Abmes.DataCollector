using Abmes.DataCollector.Collector.Common.Configuration;
using Abmes.DataCollector.Vault.Configuration;
using Abmes.DataCollector.Vault.Storage;
using System;
using System.Collections.Generic;
using System.Text;

namespace Abmes.DataCollector.Vault.Amazon.Storage
{
    public class AmazonStorageResolver : IStorageResolver
    {
        private readonly IAmazonStorageFactory _amazonStorageFactory;

        public AmazonStorageResolver(
            IAmazonStorageFactory amazonStorageFactory)
        {
            _amazonStorageFactory = amazonStorageFactory;
        }

        public bool CanResolve(StorageConfig storageConfig)
        {
            return string.Equals(storageConfig.StorageType, "Amazon");
        }

        public IStorage GetStorage(StorageConfig storageConfig)
        {
            return _amazonStorageFactory(storageConfig);
        }
    }
}
