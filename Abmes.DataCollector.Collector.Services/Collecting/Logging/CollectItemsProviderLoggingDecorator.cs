using Abmes.DataCollector.Collector.Common.Identity;
using Abmes.DataCollector.Utils;
using Microsoft.Extensions.Logging;

namespace Abmes.DataCollector.Collector.Services.Collecting.Logging;

public class CollectItemsProviderLoggingDecorator(
    ILogger<CollectItemsProviderLoggingDecorator> logger,
    ICollectItemsProvider collectItemsProvider) : ICollectItemsProvider
{
    public async Task<IEnumerable<CollectItem>> GetCollectItemsAsync(
        string dataCollectionName,
        string? collectFileIdentifiersUrl,
        IEnumerable<KeyValuePair<string, string>> collectFileIdentifiersHeaders,
        string collectUrl,
        IEnumerable<KeyValuePair<string, string>> collectHeaders,
        IdentityServiceClientInfo? identityServiceClientInfo,
        CancellationToken cancellationToken)
    {
        try
        {
            logger.LogInformation("Started getting collect items for data collection '{dataCollectionName}'", dataCollectionName);

            var result = 
                    (await collectItemsProvider.GetCollectItemsAsync(dataCollectionName, collectFileIdentifiersUrl, collectFileIdentifiersHeaders, collectUrl, collectHeaders, identityServiceClientInfo, cancellationToken))
                    .OrderBy(x => x.CollectFileInfo?.Name)  // todo: logging decorator should not alter behavior i.e. change the order of the result list
                    .ToList();
            
            logger.LogInformation("Finished getting collect items for data collection '{dataCollectionName}'", dataCollectionName);

            logger.LogInformation($"Retrieved {result.Count} collect items for data collection '{dataCollectionName}'", dataCollectionName);

            foreach (var collectItem in result)
            {
                logger.LogInformation(collectItem.CollectFileInfo?.Name);
            }

            return result;
        }
        catch (Exception e)
        {
            logger.LogCritical("Error getting collect items for data collection '{dataCollectionName}': {errorMessage}", dataCollectionName, e.GetAggregateMessages());
            logger.LogCritical(e.StackTrace);
            throw;
        }
    }

    public async Task<IEnumerable<CollectItem>> GetRedirectedCollectItemsAsync(
        IEnumerable<CollectItem> collectItems,
        string dataCollectionName,
        IEnumerable<KeyValuePair<string, string>> collectHeaders,
        int maxDegreeOfParallelism,
        IdentityServiceClientInfo? identityServiceClientInfo,
        CancellationToken cancellationToken)
    {
        try
        {
            logger.LogInformation("Started getting redirected collect urls for data collection '{dataCollectionName}'", dataCollectionName);

            var result = await collectItemsProvider.GetRedirectedCollectItemsAsync(collectItems, dataCollectionName, collectHeaders, maxDegreeOfParallelism, identityServiceClientInfo, cancellationToken);

            logger.LogInformation("Finished getting redirected collect urls for data collection '{dataCollectionName}'", dataCollectionName);

            return result;
        }
        catch (Exception e)
        {
            logger.LogCritical("Error getting collect redirected urls for data collection '{dataCollectionName}': {errorMessage}", dataCollectionName, e.GetAggregateMessages());
            throw;
        }
    }
}
