using Abmes.DataCollector.Collector.Common.Collecting;
using Abmes.DataCollector.Utils;
using Microsoft.Extensions.Logging;

namespace Abmes.DataCollector.Collector.Logging.Collecting;

public class CollectUrlExtractor : ICollectUrlExtractor
{
    private readonly ILogger<CollectUrlExtractor> _logger;
    private readonly ICollectUrlExtractor _collectUrlExtractor;

    public CollectUrlExtractor(
        ILogger<CollectUrlExtractor> logger,
        ICollectUrlExtractor collectUrlExtractor)
    {
        _logger = logger;
        _collectUrlExtractor = collectUrlExtractor;
    }

    public async Task<string> ExtractCollectUrlAsync(string dataCollectionName, string? collectFileIdentifier, string sourceUrl, IEnumerable<KeyValuePair<string, string>> headers, string? identityServiceAccessToken, CancellationToken cancellationToken)
    {
        collectFileIdentifier ??= "default";

        try
        {
            _logger.LogInformation("Started getting collect url for file '{collectFileIdentifier}' in data collection '{dataCollectionName}'", collectFileIdentifier, dataCollectionName);

            var result = await _collectUrlExtractor.ExtractCollectUrlAsync(dataCollectionName, collectFileIdentifier, sourceUrl, headers, identityServiceAccessToken, cancellationToken);

            _logger.LogInformation("Finished getting collect url for file '{collectFileIdentifier}' in data collection '{dataCollectionName}'", collectFileIdentifier, dataCollectionName);

            return result;
        }
        catch (Exception e)
        {
            _logger.LogCritical("Error getting collect url for file '{collectFileIdentifier}' in data collection '{dataCollectionName}': {errorMessage}", collectFileIdentifier, dataCollectionName, e.GetAggregateMessages());
            throw;
        }
    }
}
