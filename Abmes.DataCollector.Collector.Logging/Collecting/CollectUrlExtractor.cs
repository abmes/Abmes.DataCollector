﻿using Abmes.DataCollector.Collector.Common.Collecting;
using Abmes.DataCollector.Utils;
using Microsoft.Extensions.Logging;

namespace Abmes.DataCollector.Collector.Logging.Collecting;

public class CollectUrlExtractor(
    ILogger<CollectUrlExtractor> logger,
    ICollectUrlExtractor collectUrlExtractor) : ICollectUrlExtractor
{
    public async Task<string> ExtractCollectUrlAsync(string dataCollectionName, string? collectFileIdentifier, string sourceUrl, IEnumerable<KeyValuePair<string, string>> headers, string? identityServiceAccessToken, CancellationToken cancellationToken)
    {
        collectFileIdentifier ??= "default";

        try
        {
            logger.LogInformation("Started getting collect url for file '{collectFileIdentifier}' in data collection '{dataCollectionName}'", collectFileIdentifier, dataCollectionName);

            var result = await collectUrlExtractor.ExtractCollectUrlAsync(dataCollectionName, collectFileIdentifier, sourceUrl, headers, identityServiceAccessToken, cancellationToken);

            logger.LogInformation("Finished getting collect url for file '{collectFileIdentifier}' in data collection '{dataCollectionName}'", collectFileIdentifier, dataCollectionName);

            return result;
        }
        catch (Exception e)
        {
            logger.LogCritical("Error getting collect url for file '{collectFileIdentifier}' in data collection '{dataCollectionName}': {errorMessage}", collectFileIdentifier, dataCollectionName, e.GetAggregateMessages());
            throw;
        }
    }
}
