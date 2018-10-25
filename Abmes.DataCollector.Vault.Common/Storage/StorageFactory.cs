using Autofac.Features.Indexed;
using Abmes.DataCollector.Vault.Configuration;
using System;
using System.Collections.Generic;
using System.Text;

namespace Abmes.DataCollector.Vault.Storage
{
    public class StorageFactory : IStorageFactory
    {
        private readonly IIndex<string, IStorage> _factory;

        public StorageFactory(IIndex<string, IStorage> factory)
        {
            _factory = factory;
        }

        public IStorage GetStorage(StorageConfig storageConfig)
        {
            var result = _factory[storageConfig.StorageType];
            result.StorageConfig = storageConfig;

            return result;
        }
    }
}
