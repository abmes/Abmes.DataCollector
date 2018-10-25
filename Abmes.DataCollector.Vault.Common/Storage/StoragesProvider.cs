using Abmes.DataCollector.Vault.Configuration;
using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Collections.Generic;

namespace Abmes.DataCollector.Vault.Storage
{
    public class StoragesProvider : IStoragesProvider
    {
        private readonly IStorageConfigsProvider _storageConfigProvider;
        private readonly IStorageFactory _storageFactory;

        public StoragesProvider(
            IStorageConfigsProvider storageConfigProvider,
            IStorageFactory storageFactory)
        {
            _storageConfigProvider = storageConfigProvider;
            _storageFactory = storageFactory;
        }

        public async Task<IEnumerable<IStorage>> GetStoragesAsync(CancellationToken cancellationToken)
        {
            var storageConfig = await _storageConfigProvider.GetStorageConfigsAsync(cancellationToken);
            return storageConfig.Select(x => _storageFactory.GetStorage(x));
        }
    }
}
