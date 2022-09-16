using Abmes.DataCollector.Collector.Common.Collecting;
using Abmes.DataCollector.Collector.Common.Configuration;
using Abmes.DataCollector.Common.Storage;
using Abmes.DataCollector.Utils;
using Microsoft.Extensions.Logging;

namespace Abmes.DataCollector.Collector.Logging.Collecting;

public class CollectItemsProvider : ICollectItemsProvider
{
    private readonly ILogger<CollectItemsProvider> _logger;
    private readonly ICollectItemsProvider _collectItemsProvider;

    public CollectItemsProvider(
        ILogger<CollectItemsProvider> logger,
        ICollectItemsProvider collectItemsProvider)
    {
        _logger = logger;
        _collectItemsProvider = collectItemsProvider;
    }

    public IEnumerable<(FileInfoData CollectFileInfo, string CollectUrl)> GetCollectItems(string dataCollectionName, string? collectFileIdentifiersUrl, IEnumerable<KeyValuePair<string, string>> collectFileIdentifiersHeaders, string collectUrl, IEnumerable<KeyValuePair<string, string>> collectHeaders, IdentityServiceClientInfo identityServiceClientInfo, CancellationToken cancellationToken)
    {
        try
        {
            _logger.LogInformation("Started getting collect items for data collection '{dataCollectionName}'", dataCollectionName);

            var result = 
                    _collectItemsProvider.GetCollectItems(dataCollectionName, collectFileIdentifiersUrl, collectFileIdentifiersHeaders, collectUrl, collectHeaders, identityServiceClientInfo, cancellationToken)
                    .OrderBy(x => x.CollectFileInfo?.Name)
                    .ToList();

            _logger.LogInformation("Finished getting collect items for data collection '{dataCollectionName}'", dataCollectionName);

            _logger.LogInformation($"Retrieved {result.Count} collect items for data collection '{dataCollectionName}'", dataCollectionName);

            foreach (var collectItem in result)
            {
                _logger.LogInformation(collectItem.CollectFileInfo?.Name);
            }

            return result;
        }
        catch (Exception e)
        {
            _logger.LogCritical("Error getting collect items for data collection '{dataCollectionName}': {errorMessage}", dataCollectionName, e.GetAggregateMessages());
            throw;
        }
    }

    public async Task<IEnumerable<(FileInfoData CollectFileInfo, string CollectUrl)>> GetRedirectedCollectItemsAsync(IEnumerable<(FileInfoData CollectFileInfo, string CollectUrl)> collectItems, string dataCollectionName, IEnumerable<KeyValuePair<string, string>> collectHeaders, int maxDegreeOfParallelism, IdentityServiceClientInfo identityServiceClientInfo, CancellationToken cancellationToken)
    {
        try
        {
            _logger.LogInformation("Started getting redirected collect urls for data collection '{dataCollectionName}'", dataCollectionName);

            var result = await _collectItemsProvider.GetRedirectedCollectItemsAsync(collectItems, dataCollectionName, collectHeaders, maxDegreeOfParallelism, identityServiceClientInfo, cancellationToken);

            _logger.LogInformation("Finished getting redirected collect urls for data collection '{dataCollectionName}'", dataCollectionName);

            return result;
        }
        catch (Exception e)
        {
            _logger.LogCritical("Error getting collect redirected urls for data collection '{dataCollectionName}': {errorMessage}", dataCollectionName, e.GetAggregateMessages());
            throw;
        }
    }
}
