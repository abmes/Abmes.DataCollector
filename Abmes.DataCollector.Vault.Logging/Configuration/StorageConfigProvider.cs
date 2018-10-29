using Microsoft.Extensions.Logging;
using Abmes.DataCollector.Vault.Configuration;
using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace Abmes.DataCollector.Vault.Logging.Configuration
{
    public class StorageConfigProvider : IStoragesConfigProvider
    {
        private readonly ILogger<IStoragesConfigProvider> _logger;
        private readonly IStoragesConfigProvider _destinationsConfigProvider;

        public StorageConfigProvider(ILogger<IStoragesConfigProvider> logger, IStoragesConfigProvider destinationsConfigProvider)
        {
            _logger = logger;
            _destinationsConfigProvider = destinationsConfigProvider;
        }
        public async Task<IEnumerable<StorageConfig>> GetStorageConfigsAsync(CancellationToken cancellationToken)
        {
            _logger.LogTrace("Started getting storage config");

            var result = await _destinationsConfigProvider.GetStorageConfigsAsync(cancellationToken);

            _logger.LogTrace("Finished getting storage config");

            return result;
        }
    }
}
