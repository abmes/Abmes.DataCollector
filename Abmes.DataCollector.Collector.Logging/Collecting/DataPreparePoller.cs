using Microsoft.Extensions.Logging;
using Abmes.DataCollector.Collector.Common.Collecting;
using Abmes.DataCollector.Utils;

namespace Abmes.DataCollector.Collector.Logging.Collecting;

public class DatabPreparePoller : IDataPreparePoller
{
    private readonly ILogger<DatabPreparePoller> _logger;
    private readonly IDataPreparePoller _dataPreparePoller;

    public DatabPreparePoller(ILogger<DatabPreparePoller> logger, IDataPreparePoller dataPreparePoller)
    {
        _logger = logger;
        _dataPreparePoller = dataPreparePoller;
    }

    public async Task<DataPrepareResult> GetDataPrepareResultAsync(string pollUrl, IEnumerable<KeyValuePair<string, string>> pollHeaders, CancellationToken cancellationToken)
    {
        var dataCollectionName = GetDataCollectionName(pollUrl);

        try
        {
            _logger.LogInformation("Started prepare status polling for data '{dataCollectionName}'", dataCollectionName);

            var result = await _dataPreparePoller.GetDataPrepareResultAsync(pollUrl, pollHeaders, cancellationToken);

            _logger.LogInformation("Finished prepare status polling for data '{dataCollectionName}'", dataCollectionName);

            var status = (result.Finished ? "Finished" : "Not finished");
            _logger.LogInformation("Prepare status for data '{dataCollectionName}': {status}", dataCollectionName, status);

            return result;
        }
        catch (Exception e)
        {
            _logger.LogError("Error prepare status polling for data '{dataCollectionName}': {errorMessage}", dataCollectionName, e.GetAggregateMessages());
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
            _logger.LogCritical($"Error getting data name for prepare status polling: {pollUrl}; {e.GetAggregateMessages()}");
            return "<unknown>";
        }
    }
}
