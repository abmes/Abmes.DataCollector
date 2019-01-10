using Abmes.DataCollector.Utils;
using Abmes.DataCollector.Vault.Services;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace Abmes.DataCollector.Vault.Logging.Services
{
    public class DataCollectionFiles : IDataCollectionFiles
    {
        private readonly ILogger<DataCollectionFiles> _logger;
        private readonly IDataCollectionFiles _dataCollectionFiles;

        public DataCollectionFiles(ILogger<DataCollectionFiles> logger, IDataCollectionFiles dataCollectionFiles)
        {
            _logger = logger;
            _dataCollectionFiles = dataCollectionFiles;
        }

        public async Task<string> GetDownloadUrlAsync(string fileName, CancellationToken cancellationToken)
        {
            try
            {
                _logger.LogInformation("Started getting download url for file '{fileName}'", fileName);

                var result = await _dataCollectionFiles.GetDownloadUrlAsync(fileName, cancellationToken);

                _logger.LogInformation("Finished getting download url for file '{fileName}'", fileName);

                return result;
            }
            catch (Exception e)
            {
                _logger.LogCritical("Error getting download url for file '{fileName}': {errorMessage}", fileName, e.GetAggregateMessages());
                throw;
            }
        }

        public async Task<IEnumerable<string>> GetDownloadUrlsAsync(string fileNamePrefix, CancellationToken cancellationToken)
        {
            try
            {
                _logger.LogInformation("Started getting download urls for file name prefix'{fileNamePrefix}'", fileNamePrefix);

                var result = await _dataCollectionFiles.GetDownloadUrlsAsync(fileNamePrefix, cancellationToken);

                _logger.LogInformation("Finished getting download urls for file name prefix '{fileNamePrefix}'", fileNamePrefix);

                return result;
            }
            catch (Exception e)
            {
                _logger.LogCritical("Error getting download urls for file name prefix '{fileNamePrefix}': {errorMessage}", fileNamePrefix, e.GetAggregateMessages());
                throw;
            }
        }

        public async Task<IEnumerable<string>> GetFileNamesAsync(string prefix, CancellationToken cancellationToken)
        {
            try
            {
                _logger.LogInformation("Started getting file names");

                var result = await _dataCollectionFiles.GetFileNamesAsync(prefix, cancellationToken);

                _logger.LogInformation("Finished getting file names");

                return result;
            }
            catch (Exception e)
            {
                _logger.LogCritical("Error getting file names: {errorMessage}", e.GetAggregateMessages());
                throw;
            }
        }

        public async Task<IEnumerable<string>> GetLatestDownloadUrlsAsync(CancellationToken cancellationToken)
        {
            try
            {
                _logger.LogInformation("Started getting latest download url");

                var result = await _dataCollectionFiles.GetLatestDownloadUrlsAsync(cancellationToken);

                _logger.LogInformation("Finished getting latest download url");

                return result;
            }
            catch (Exception e)
            {
                _logger.LogCritical("Error getting latest download url: {errorMessage}", e.GetAggregateMessages());
                throw;
            }
        }

        public async Task<IEnumerable<string>> GetLatestFileNamesAsync(CancellationToken cancellationToken)
        {
            try
            {
                _logger.LogInformation("Started getting latest file name");

                var result = await _dataCollectionFiles.GetLatestFileNamesAsync(cancellationToken);

                _logger.LogInformation("Finished getting latest file name");

                return result;
            }
            catch (Exception e)
            {
                _logger.LogCritical("Error getting latest file name: {errorMessage}", e.GetAggregateMessages());
                throw;
            }
        }
    }
}
