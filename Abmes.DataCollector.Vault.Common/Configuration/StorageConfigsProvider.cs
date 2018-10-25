using Abmes.DataCollector.Collector.Common.Configuration;
using Abmes.DataCollector.Vault.Configuration;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Abmes.DataCollector.Vault.Common.Configuration
{
    public class StorageConfigsProvider : IStorageConfigsProvider
    {
        private const string StorageConfigName = "StorageConfig.json";

        private readonly IStorageJsonConfigsProvider _storageJsonConfigProvider;
        private readonly IConfigProvider _configProvider;

        public StorageConfigsProvider(
            IStorageJsonConfigsProvider storageJsonConfigProvider,
            IConfigProvider configProvider)
        {
            _storageJsonConfigProvider = storageJsonConfigProvider;
            _configProvider = configProvider;
        }

        public async Task<IEnumerable<StorageConfig>> GetStorageConfigsAsync(CancellationToken cancellationToken)
        {
            var json = await _configProvider.GetConfigContentAsync(StorageConfigName, cancellationToken);
            return _storageJsonConfigProvider.GetStorageConfigs(json);
        }
    }
}
