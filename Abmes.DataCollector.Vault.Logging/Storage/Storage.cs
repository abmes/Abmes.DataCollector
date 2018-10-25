using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using System.Threading;
using Abmes.DataCollector.Vault.Storage;
using Abmes.DataCollector.Vault.Configuration;
using Abmes.DataCollector.Utils;

namespace Abmes.DataCollector.Vault.Logging.Storage
{
    public class Storage : IStorage
    {
        private readonly ILogger<Storage> _logger;
        private readonly IStorage _storage;

        public Storage(ILogger<Storage> logger, IStorage storage)
        {
            _logger = logger;
            _storage = storage;
        }

        public StorageConfig StorageConfig
        {
            get => _storage.StorageConfig;
            set
            {
                _storage.StorageConfig = value;
            }
        }

        public async Task<IEnumerable<string>> GetDataCollectionFileNamesAsync(string dataCollectionName, CancellationToken cancellationToken)
        {
            try
            {
                _logger.LogInformation("Started getting data '{dataCollectionName}' file names", dataCollectionName);

                var result =  await _storage.GetDataCollectionFileNamesAsync(dataCollectionName, cancellationToken);

                _logger.LogInformation("Finished getting data '{dataCollectionName}' file names", dataCollectionName);

                return result;
            }
            catch (Exception e)
            {
                _logger.LogCritical("Error getting data '{dataCollectionName}' file names: {errorMessage}", dataCollectionName, e.GetAggregateMessages());
                throw;
            }
        }

        public async Task<string> GetDataCollectionFileDownloadUrlAsync(string dataCollectionName, string fileName, CancellationToken cancellationToken)
        {
            try
            {
                _logger.LogInformation("Started getting download url for file '{fileName}' in data '{dataCollectionName}'", fileName, dataCollectionName);

                var result = await _storage.GetDataCollectionFileDownloadUrlAsync(dataCollectionName, fileName, cancellationToken);

                _logger.LogInformation("Finished getting download url for file '{fileName}' in data '{dataCollectionName}'", fileName, dataCollectionName);

                return result;
            }
            catch (Exception e)
            {
                _logger.LogCritical("Error getting download url for file '{fileName}' in data '{dataCollectionName}': {errorMessage}", fileName, dataCollectionName, e.GetAggregateMessages());
                throw;
            }
        }

    }
}
