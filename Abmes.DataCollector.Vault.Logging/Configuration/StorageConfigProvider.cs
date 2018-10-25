using Microsoft.Extensions.Logging;
using Abmes.DataCollector.Vault.Configuration;
using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace Abmes.DataCollector.Vault.Logging.Configuration
{
    public class StorageConfigProvider : IStorageConfigsProvider
    {
        private readonly ILogger<IStorageConfigsProvider> _logger;
        private readonly IStorageConfigsProvider _destinationsConfigProvider;

        public StorageConfigProvider(ILogger<IStorageConfigsProvider> logger, IStorageConfigsProvider destinationsConfigProvider)
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
