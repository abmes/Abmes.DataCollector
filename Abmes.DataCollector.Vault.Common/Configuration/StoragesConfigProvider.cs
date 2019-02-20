using Abmes.DataCollector.Common.Configuration;
using Abmes.DataCollector.Vault.Configuration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Abmes.DataCollector.Vault.Common.Configuration
{
    public class StoragesConfigProvider : IStoragesConfigProvider
    {
        private const string StorageConfigName = "StoragesConfig.json";

        private readonly IStoragesJsonConfigProvider _storageJsonConfigProvider;
        private readonly IConfigProvider _configProvider;

        public StoragesConfigProvider(
            IStoragesJsonConfigProvider storageJsonConfigProvider,
            IConfigProvider configProvider)
        {
            _storageJsonConfigProvider = storageJsonConfigProvider;
            _configProvider = configProvider;
        }

        public async Task<IEnumerable<StorageConfig>> GetStorageConfigsAsync(CancellationToken cancellationToken)
        {
            var json = await _configProvider.GetConfigContentAsync(StorageConfigName, cancellationToken);
            var result = _storageJsonConfigProvider.GetStorageConfigs(json);

            if (result.Any(x => string.IsNullOrEmpty(x.StorageType)))
            {
                throw new Exception("Invalid StorageType");
            }

            return result;
        }
    }
}
