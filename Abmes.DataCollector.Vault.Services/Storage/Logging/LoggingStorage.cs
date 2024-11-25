using Abmes.DataCollector.Common;
using Abmes.DataCollector.Utils;
using Abmes.DataCollector.Vault.Services.Ports.Configuration;
using Abmes.DataCollector.Vault.Services.Ports.Storage;
using Microsoft.Extensions.Logging;

namespace Abmes.DataCollector.Vault.Services.Storage.Logging;

public class LoggingStorage(
    IStorage storage,
    ILogger<LoggingStorage> logger) : ILoggingStorage
{
    public StorageConfig StorageConfig => storage.StorageConfig;

    public async Task<IEnumerable<string>> GetDataCollectionFileNamesAsync(string dataCollectionName, string? fileNamePrefix, CancellationToken cancellationToken)
    {
        try
        {
            logger.LogInformation("Started getting data '{dataCollectionName}' file names", dataCollectionName);

            var result = await storage.GetDataCollectionFileNamesAsync(dataCollectionName, fileNamePrefix, cancellationToken);

            logger.LogInformation("Finished getting data '{dataCollectionName}' file names", dataCollectionName);

            return result;
        }
        catch (Exception e)
        {
            logger.LogCritical("Error getting data '{dataCollectionName}' file names: {errorMessage}", dataCollectionName, e.GetAggregateMessages());
            throw;
        }
    }

    public async Task<IEnumerable<FileInfoData>> GetDataCollectionFileInfosAsync(string dataCollectionName, string? fileNamePrefix, CancellationToken cancellationToken)
    {
        try
        {
            logger.LogInformation("Started getting data '{dataCollectionName}' file infos", dataCollectionName);

            var result = await storage.GetDataCollectionFileInfosAsync(dataCollectionName, fileNamePrefix, cancellationToken);

            logger.LogInformation("Finished getting data '{dataCollectionName}' file infos", dataCollectionName);

            return result;
        }
        catch (Exception e)
        {
            logger.LogCritical("Error getting data '{dataCollectionName}' file infos: {errorMessage}", dataCollectionName, e.GetAggregateMessages());
            throw;
        }
    }

    public async Task<string> GetDataCollectionFileDownloadUrlAsync(string dataCollectionName, string fileName, CancellationToken cancellationToken)
    {
        try
        {
            logger.LogInformation("Started getting download url for file '{fileName}' in data '{dataCollectionName}'", fileName, dataCollectionName);

            var result = await storage.GetDataCollectionFileDownloadUrlAsync(dataCollectionName, fileName, cancellationToken);

            logger.LogInformation("Finished getting download url for file '{fileName}' in data '{dataCollectionName}'", fileName, dataCollectionName);

            return result;
        }
        catch (Exception e)
        {
            logger.LogCritical("Error getting download url for file '{fileName}' in data '{dataCollectionName}': {errorMessage}", fileName, dataCollectionName, e.GetAggregateMessages());
            throw;
        }
    }
}
