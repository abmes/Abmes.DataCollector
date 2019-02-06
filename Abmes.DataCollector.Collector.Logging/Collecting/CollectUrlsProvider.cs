using Abmes.DataCollector.Collector.Common.Collecting;
using Abmes.DataCollector.Utils;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;

namespace Abmes.DataCollector.Collector.Logging.Collecting
{
    public class CollectUrlsProvider : ICollectUrlsProvider
    {
        private readonly ILogger<CollectUrlsProvider> _logger;
        private readonly ICollectUrlsProvider _collectUrlsProvider;

        public CollectUrlsProvider(
            ILogger<CollectUrlsProvider> logger,
            ICollectUrlsProvider collectUrlsProvider)
        {
            _logger = logger;
            _collectUrlsProvider = collectUrlsProvider;
        }

        public IEnumerable<string> GetCollectUrls(string dataCollectionName, string collectFileIdentifiersUrl, IEnumerable<KeyValuePair<string, string>> collectFileIdentifiersHeaders, string collectUrl, IEnumerable<KeyValuePair<string, string>> collectHeaders, int maxDegreeOfParallelism, CancellationToken cancellationToken)
        {
            try
            {
                _logger.LogInformation("Started getting collect urls for data collection '{dataCollectionName}'", dataCollectionName);

                var result = _collectUrlsProvider.GetCollectUrls(dataCollectionName, collectFileIdentifiersUrl, collectFileIdentifiersHeaders, collectUrl, collectHeaders, maxDegreeOfParallelism, cancellationToken);

                _logger.LogInformation("Finished getting collect urls for data collection '{dataCollectionName}'", dataCollectionName);

                return result;
            }
            catch (Exception e)
            {
                _logger.LogCritical("Error getting collect urls for data collection '{dataCollectionName}': {errorMessage}", dataCollectionName, e.GetAggregateMessages());
                throw;
            }
        }
    }
}
