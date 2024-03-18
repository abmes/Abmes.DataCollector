using Abmes.DataCollector.Collector.Common.Collecting;
using Microsoft.Extensions.Logging;
using Abmes.DataCollector.Utils;
using Abmes.DataCollector.Collector.Common.Configuration;

namespace Abmes.DataCollector.Collector.Logging.Collecting;

public class DataPreparer(
    ILogger<DataPreparer> logger,
    IDataPreparer dataPreparer) : IDataPreparer
{
    public async Task<bool> PrepareDataAsync(DataCollectionConfig dataCollectionConfig, CancellationToken cancellationToken)
    {
        try
        {
            if (!string.IsNullOrEmpty(dataCollectionConfig.PrepareUrl))
            {
                logger.LogInformation("Started preparing data '{dataCollectionName}'", dataCollectionConfig.DataCollectionName);
            }

            var result = await dataPreparer.PrepareDataAsync(dataCollectionConfig, cancellationToken);

            if (!string.IsNullOrEmpty(dataCollectionConfig.PrepareUrl))
            {
                logger.LogInformation("Finished preparing data '{dataCollectionName}'", dataCollectionConfig.DataCollectionName);
            }

            return result;
        }
        catch (Exception e)
        {
            logger.LogCritical("Error preparing data '{dataCollectionName}': {errorMessage}", dataCollectionConfig.DataCollectionName, e.GetAggregateMessages());
            throw;
        }
    }
}
