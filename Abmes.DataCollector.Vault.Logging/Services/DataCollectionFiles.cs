using Abmes.DataCollector.Common.Storage;
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

        public async Task<string> GetDownloadUrlAsync(string fileName, string storageType = default, CancellationToken cancellationToken = default)
        {
            try
            {
                _logger.LogInformation("Started getting download url" + FromStorageType(storageType) + " for file '{fileName}'", fileName);

                var result = await _dataCollectionFiles.GetDownloadUrlAsync(fileName, storageType, cancellationToken);

                _logger.LogInformation("Finished getting download url" + FromStorageType(storageType) + " for file '{fileName}'", fileName);

                return result;
            }
            catch (Exception e)
            {
                _logger.LogCritical("Error getting download url" + FromStorageType(storageType) + " for file '{fileName}': {errorMessage}", fileName, e.GetAggregateMessages());
                throw;
            }
        }

        public async Task<IEnumerable<string>> GetDownloadUrlsAsync(string fileNamePrefix, string storageType = default, CancellationToken cancellationToken = default)
        {
            try
            {
                _logger.LogInformation("Started getting download urls" + FromStorageType(storageType) + " for file name prefix'{fileNamePrefix}'", fileNamePrefix);

                var result = await _dataCollectionFiles.GetDownloadUrlsAsync(fileNamePrefix, storageType, cancellationToken);

                _logger.LogInformation("Finished getting download urls" + FromStorageType(storageType) + " for file name prefix '{fileNamePrefix}'", fileNamePrefix);

                return result;
            }
            catch (Exception e)
            {
                _logger.LogCritical("Error getting download urls" + FromStorageType(storageType) + " for file name prefix '{fileNamePrefix}': {errorMessage}", fileNamePrefix, e.GetAggregateMessages());
                throw;
            }
        }

        public async Task<IEnumerable<IFileInfo>> GetFileInfosAsync(string prefix, TimeSpan? maxAge, CancellationToken cancellationToken)
        {
            try
            {
                _logger.LogInformation("Started getting file infos");

                var result = await _dataCollectionFiles.GetFileInfosAsync(prefix, maxAge, cancellationToken);

                _logger.LogInformation("Finished getting file infos");

                return result;
            }
            catch (Exception e)
            {
                _logger.LogCritical("Error getting file infos: {errorMessage}", e.GetAggregateMessages());
                throw;
            }
        }

        public async Task<IEnumerable<string>> GetFileNamesAsync(string prefix, TimeSpan? maxAge, CancellationToken cancellationToken)
        {
            try
            {
                _logger.LogInformation("Started getting file names");

                var result = await _dataCollectionFiles.GetFileNamesAsync(prefix, maxAge, cancellationToken);

                _logger.LogInformation("Finished getting file names");

                return result;
            }
            catch (Exception e)
            {
                _logger.LogCritical("Error getting file names: {errorMessage}", e.GetAggregateMessages());
                throw;
            }
        }

        public async Task<IEnumerable<string>> GetLatestDownloadUrlsAsync(string storageType = default, CancellationToken cancellationToken = default)
        {
            try
            {
                _logger.LogInformation("Started getting latest download url" + FromStorageType(storageType));

                var result = await _dataCollectionFiles.GetLatestDownloadUrlsAsync(storageType, cancellationToken);

                _logger.LogInformation("Finished getting latest download url" + FromStorageType(storageType));

                return result;
            }
            catch (Exception e)
            {
                _logger.LogCritical("Error getting latest download url" + FromStorageType(storageType) + ": {errorMessage}", e.GetAggregateMessages());
                throw;
            }
        }

        public async Task<IEnumerable<IFileInfo>> GetLatestFileInfosAsync(CancellationToken cancellationToken)
        {
            try
            {
                _logger.LogInformation("Started getting latest file infos");

                var result = await _dataCollectionFiles.GetLatestFileInfosAsync(cancellationToken);

                _logger.LogInformation("Finished getting latest file infos");

                return result;
            }
            catch (Exception e)
            {
                _logger.LogCritical("Error getting latest file infos: {errorMessage}", e.GetAggregateMessages());
                throw;
            }
        }

        public async Task<IEnumerable<string>> GetLatestFileNamesAsync(CancellationToken cancellationToken)
        {
            try
            {
                _logger.LogInformation("Started getting latest file names");

                var result = await _dataCollectionFiles.GetLatestFileNamesAsync(cancellationToken);

                _logger.LogInformation("Finished getting latest file names");

                return result;
            }
            catch (Exception e)
            {
                _logger.LogCritical("Error getting latest file names: {errorMessage}", e.GetAggregateMessages());
                throw;
            }
        }

        private string FromStorageType(string storageType)
        {
            return (string.IsNullOrEmpty(storageType) ? null : " from " + storageType);
        }
    }
}
