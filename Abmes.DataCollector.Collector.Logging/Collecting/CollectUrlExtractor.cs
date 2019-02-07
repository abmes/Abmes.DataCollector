using Abmes.DataCollector.Collector.Common.Collecting;
using Abmes.DataCollector.Utils;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;

namespace Abmes.DataCollector.Collector.Logging.Collecting
{
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

        public string ExtractCollectUrl(string dataCollectionName, string collectFileIdentifier, string sourceUrl, IEnumerable<KeyValuePair<string, string>> headers, CancellationToken cancellationToken)
        {
            try
            {
                _logger.LogInformation("Started getting collect url for file '{collectFileIdentifier}' in data collection '{dataCollectionName}'", collectFileIdentifier, dataCollectionName);

                var result = _collectUrlExtractor.ExtractCollectUrl(dataCollectionName, collectFileIdentifier, sourceUrl, headers, cancellationToken);

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
}
