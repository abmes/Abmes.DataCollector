using Abmes.Utils;
using Microsoft.Extensions.Logging;

namespace Abmes.DataCollector.Collector.Data.Http.Collecting.Logging;

public class DataPreparePollerLoggingDecorator(
    ILogger<DataPreparePollerLoggingDecorator> logger,
    IDataPreparePoller dataPreparePoller)
    : IDataPreparePoller
{
    public async Task<DataPrepareResult> GetDataPrepareResultAsync(string pollUrl, IEnumerable<KeyValuePair<string, string>> pollHeaders, CancellationToken cancellationToken)
    {
        var dataCollectionName = GetDataCollectionName(pollUrl);

        try
        {
            logger.LogInformation("Started prepare status polling for data '{dataCollectionName}'", dataCollectionName);

            var result = await dataPreparePoller.GetDataPrepareResultAsync(pollUrl, pollHeaders, cancellationToken);

            logger.LogInformation("Finished prepare status polling for data '{dataCollectionName}'", dataCollectionName);

            var status =
                result.Finished
                ? "Finished"
                : "Not finished";

            logger.LogInformation("Prepare status for data '{dataCollectionName}': {status}", dataCollectionName, status);

            return result;
        }
        catch (Exception e)
        {
            logger.LogError("Error prepare status polling for data '{dataCollectionName}': {errorMessage}", dataCollectionName, e.GetAggregateMessages());
            throw;
        }
    }

    private string? GetDataCollectionName(string pollUrl)
    {
        try
        {
            var uri = new Uri(pollUrl);
            return uri.Query.TrimStart('?').Split('&').Where(x => x.StartsWith("schemaName=")).Select(x => x.Split('=')[1]).FirstOrDefault();
        }
        catch (Exception e)
        {
            logger.LogCritical("Error getting data name for prepare status polling: {pollUrl}; {aggregateMessages}", pollUrl, e.GetAggregateMessages());
            return "<unknown>";
        }
    }
}
