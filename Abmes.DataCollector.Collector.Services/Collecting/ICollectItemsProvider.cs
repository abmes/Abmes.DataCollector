using Abmes.DataCollector.Collector.Common.Identity;

namespace Abmes.DataCollector.Collector.Services.Collecting;

public interface ICollectItemsProvider
{
    Task<IEnumerable<CollectItem>> GetCollectItemsAsync(
        string dataCollectionName,
        string? collectFileIdentifiersUrl,
        IEnumerable<KeyValuePair<string, string>> collectFileIdentifiersHeaders,
        string collectUrl,
        IEnumerable<KeyValuePair<string, string>> collectHeaders,
        IdentityServiceClientInfo? identityServiceClientInfo,
        CancellationToken cancellationToken);

    Task<IEnumerable<CollectItem>> GetRedirectedCollectItemsAsync(
        IEnumerable<CollectItem> collectItems,
        string dataCollectionName,
        IEnumerable<KeyValuePair<string, string>> collectHeaders,
        int maxDegreeOfParallelism,
        IdentityServiceClientInfo? identityServiceClientInfo,
        CancellationToken cancellationToken);
}
