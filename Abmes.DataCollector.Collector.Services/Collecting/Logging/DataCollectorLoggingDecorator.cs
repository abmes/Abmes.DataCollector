using Abmes.DataCollector.Collector.Services.Ports.AppConfig;
using Abmes.DataCollector.Collector.Services.Ports.Configuration;
using Abmes.DataCollector.Common;
using Abmes.DataCollector.Utils;
using Microsoft.Extensions.Logging;

namespace Abmes.DataCollector.Collector.Services.Collecting.Logging;

public class DataCollectorLoggingDecorator(
    ILogger<DataCollectorLoggingDecorator> logger,
    IDataCollector dataCollector) : IDataCollector
{
    public async Task<(IEnumerable<string> NewFileNames, IEnumerable<FileInfoData> CollectionFileInfos)> CollectDataAsync(CollectorMode collectorMode, DataCollectionConfig dataCollectionConfig, CancellationToken cancellationToken)
    {
        try
        {
            (IEnumerable<string> NewFileNames, IEnumerable<FileInfoData> CollectionFileInfos) result;

            logger.LogInformation("Started processing data '{dataCollectionName}'", dataCollectionConfig.DataCollectionName);

            var watch = System.Diagnostics.Stopwatch.StartNew();
            try
            {
                result = await dataCollector.CollectDataAsync(collectorMode, dataCollectionConfig, cancellationToken);
                result = (result.NewFileNames.ToList(), result.CollectionFileInfos.ToList());
            }
            finally
            {
                watch.Stop();
            }

            logger.LogInformation("Finished processing data '{dataCollectionName}'. Elapsed time: {elapsed}", dataCollectionConfig.DataCollectionName, watch.Elapsed);

            return result;
        }
        catch (Exception e)
        {
            logger.LogCritical("Error processing data '{dataCollectionName}': {errorMessage}", dataCollectionConfig.DataCollectionName, e.GetAggregateMessages());
            throw;
        }
    }

    public async Task GarbageCollectDataAsync(DataCollectionConfig dataCollectionConfig, IEnumerable<string> newFileNames, IEnumerable<FileInfoData> collectionFileInfos, CancellationToken cancellationToken)
    {
        try
        {
            await dataCollector.GarbageCollectDataAsync(dataCollectionConfig, newFileNames, collectionFileInfos, cancellationToken);
        }
        catch (Exception e)
        {
            logger.LogCritical("Error garbage collecting data '{dataCollectionName}': {errorMessage}", dataCollectionConfig.DataCollectionName, e.GetAggregateMessages());
            throw;
        }
    }
}
