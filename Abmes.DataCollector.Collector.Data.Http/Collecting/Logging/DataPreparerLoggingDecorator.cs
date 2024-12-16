using Abmes.DataCollector.Collector.Services.Ports.Collecting;
using Abmes.DataCollector.Collector.Services.Ports.Configuration;
using Abmes.Utils;
using Microsoft.Extensions.Logging;

namespace Abmes.DataCollector.Collector.Data.Http.Collecting.Logging;

public class DataPreparerLoggingDecorator(
    ILogger<DataPreparerLoggingDecorator> logger,
    IDataPreparer dataPreparer)
    : IDataPreparer
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
