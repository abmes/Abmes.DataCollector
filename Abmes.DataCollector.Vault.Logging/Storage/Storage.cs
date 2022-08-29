using Microsoft.Extensions.Logging;
using Abmes.DataCollector.Vault.Storage;
using Abmes.DataCollector.Vault.Configuration;
using Abmes.DataCollector.Utils;
using Abmes.DataCollector.Common.Storage;

namespace Abmes.DataCollector.Vault.Logging.Storage;

public class Storage : ILoggingStorage
{
    private readonly IStorage _storage;
    private readonly ILogger<Storage> _logger;

    public Storage(IStorage storage, ILogger<Storage> logger)
    {
        _storage = storage;
        _logger = logger;
    }

    public StorageConfig StorageConfig
    {
        get => _storage.StorageConfig;
    }

    public async Task<IEnumerable<string>> GetDataCollectionFileNamesAsync(string dataCollectionName, string? fileNamePrefix, CancellationToken cancellationToken)
    {
        try
        {
            _logger.LogInformation("Started getting data '{dataCollectionName}' file names", dataCollectionName);

            var result =  await _storage.GetDataCollectionFileNamesAsync(dataCollectionName, fileNamePrefix, cancellationToken);

            _logger.LogInformation("Finished getting data '{dataCollectionName}' file names", dataCollectionName);

            return result;
        }
        catch (Exception e)
        {
            _logger.LogCritical("Error getting data '{dataCollectionName}' file names: {errorMessage}", dataCollectionName, e.GetAggregateMessages());
            throw;
        }
    }

    public async Task<IEnumerable<FileInfoData>> GetDataCollectionFileInfosAsync(string dataCollectionName, string? fileNamePrefix, CancellationToken cancellationToken)
    {
        try
        {
            _logger.LogInformation("Started getting data '{dataCollectionName}' file infos", dataCollectionName);

            var result = await _storage.GetDataCollectionFileInfosAsync(dataCollectionName, fileNamePrefix, cancellationToken);

            _logger.LogInformation("Finished getting data '{dataCollectionName}' file infos", dataCollectionName);

            return result;
        }
        catch (Exception e)
        {
            _logger.LogCritical("Error getting data '{dataCollectionName}' file infos: {errorMessage}", dataCollectionName, e.GetAggregateMessages());
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
