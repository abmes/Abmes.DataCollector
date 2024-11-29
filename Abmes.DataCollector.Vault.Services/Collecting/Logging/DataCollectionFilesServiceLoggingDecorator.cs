using Abmes.DataCollector.Shared;
using Abmes.DataCollector.Utils;
using Abmes.DataCollector.Vault.Services.Contracts;
using Microsoft.Extensions.Logging;

namespace Abmes.DataCollector.Vault.Services.Collecting.Logging;

public class DataCollectionFilesServiceLoggingDecorator(
    ILogger<DataCollectionFilesServiceLoggingDecorator> logger,
    IDataCollectionFilesService dataCollectionFilesService)
    : IDataCollectionFilesService
{
    public async Task<string> GetDownloadUrlAsync(string fileName, string? storageType, CancellationToken cancellationToken)
    {
        try
        {
            logger.LogInformation("Started getting download url" + FromStorageType(storageType) + " for file '{fileName}'", fileName);

            var result = await dataCollectionFilesService.GetDownloadUrlAsync(fileName, storageType, cancellationToken);

            logger.LogInformation("Finished getting download url" + FromStorageType(storageType) + " for file '{fileName}'", fileName);

            return result;
        }
        catch (Exception e)
        {
            logger.LogCritical("Error getting download url" + FromStorageType(storageType) + " for file '{fileName}': {errorMessage}", fileName, e.GetAggregateMessages());
            throw;
        }
    }

    public async Task<IEnumerable<string>> GetDownloadUrlsAsync(string? fileNamePrefix, string? storageType, CancellationToken cancellationToken)
    {
        try
        {
            logger.LogInformation("Started getting download urls" + FromStorageType(storageType) + " for file name prefix'{fileNamePrefix}'", fileNamePrefix);

            var result = await dataCollectionFilesService.GetDownloadUrlsAsync(fileNamePrefix, storageType, cancellationToken);

            logger.LogInformation("Finished getting download urls" + FromStorageType(storageType) + " for file name prefix '{fileNamePrefix}'", fileNamePrefix);

            return result;
        }
        catch (Exception e)
        {
            logger.LogCritical("Error getting download urls" + FromStorageType(storageType) + " for file name prefix '{fileNamePrefix}': {errorMessage}", fileNamePrefix, e.GetAggregateMessages());
            throw;
        }
    }

    public async Task<IEnumerable<FileInfoData>> GetFileInfosAsync(string? prefix, TimeSpan? maxAge, CancellationToken cancellationToken)
    {
        try
        {
            logger.LogInformation("Started getting file infos");

            var result = await dataCollectionFilesService.GetFileInfosAsync(prefix, maxAge, cancellationToken);

            logger.LogInformation("Finished getting file infos");

            return result;
        }
        catch (Exception e)
        {
            logger.LogCritical("Error getting file infos: {errorMessage}", e.GetAggregateMessages());
            throw;
        }
    }

    public async Task<IEnumerable<string>> GetFileNamesAsync(string? prefix, TimeSpan? maxAge, CancellationToken cancellationToken)
    {
        try
        {
            logger.LogInformation("Started getting file names");

            var result = await dataCollectionFilesService.GetFileNamesAsync(prefix, maxAge, cancellationToken);

            logger.LogInformation("Finished getting file names");

            return result;
        }
        catch (Exception e)
        {
            logger.LogCritical("Error getting file names: {errorMessage}", e.GetAggregateMessages());
            throw;
        }
    }

    public async Task<IEnumerable<string>> GetLatestDownloadUrlsAsync(string? storageType, CancellationToken cancellationToken)
    {
        try
        {
            logger.LogInformation("Started getting latest download url" + FromStorageType(storageType));

            var result = await dataCollectionFilesService.GetLatestDownloadUrlsAsync(storageType, cancellationToken);

            logger.LogInformation("Finished getting latest download url" + FromStorageType(storageType));

            return result;
        }
        catch (Exception e)
        {
            logger.LogCritical("Error getting latest download url" + FromStorageType(storageType) + ": {errorMessage}", e.GetAggregateMessages());
            throw;
        }
    }

    public async Task<IEnumerable<FileInfoData>> GetLatestFileInfosAsync(CancellationToken cancellationToken)
    {
        try
        {
            logger.LogInformation("Started getting latest file infos");

            var result = await dataCollectionFilesService.GetLatestFileInfosAsync(cancellationToken);

            logger.LogInformation("Finished getting latest file infos");

            return result;
        }
        catch (Exception e)
        {
            logger.LogCritical("Error getting latest file infos: {errorMessage}", e.GetAggregateMessages());
            throw;
        }
    }

    public async Task<IEnumerable<string>> GetLatestFileNamesAsync(CancellationToken cancellationToken)
    {
        try
        {
            logger.LogInformation("Started getting latest file names");

            var result = await dataCollectionFilesService.GetLatestFileNamesAsync(cancellationToken);

            logger.LogInformation("Finished getting latest file names");

            return result;
        }
        catch (Exception e)
        {
            logger.LogCritical("Error getting latest file names: {errorMessage}", e.GetAggregateMessages());
            throw;
        }
    }

    private static string FromStorageType(string? storageType)
    {
        return
            string.IsNullOrEmpty(storageType)
            ? string.Empty
            : $" from {storageType}";
    }
}
