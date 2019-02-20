using Autofac.Features.Indexed;
using Abmes.DataCollector.Vault.Configuration;
using System;
using System.Collections.Generic;
using System.Text;

namespace Abmes.DataCollector.Vault.Storage
{
    public class StorageProvider : IStorageProvider
    {
        private readonly IStorageResolverProvider _storageResolverProvider;

        public StorageProvider(
            IStorageResolverProvider storageResolverProvider)
        {
            _storageResolverProvider = storageResolverProvider;
        }

        public IStorage GetStorage(StorageConfig storageConfig)
        {
            var resolver = _storageResolverProvider.GetResolver(storageConfig);
            return resolver.GetStorage(storageConfig);
        }
    }
}
