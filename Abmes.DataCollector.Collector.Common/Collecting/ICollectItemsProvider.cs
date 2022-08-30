using Abmes.DataCollector.Collector.Common.Configuration;
using Abmes.DataCollector.Common.Storage;

namespace Abmes.DataCollector.Collector.Common.Collecting;

public interface ICollectItemsProvider
{
    IEnumerable<(FileInfoData? CollectFileInfo, string CollectUrl)> GetCollectItems(
        string dataCollectionName,
        string collectFileIdentifiersUrl,
        IEnumerable<KeyValuePair<string, string>> collectFileIdentifiersHeaders,
        string collectUrl,
        IEnumerable<KeyValuePair<string, string>> collectHeaders,
        IdentityServiceClientInfo identityServiceClientInfo,
        CancellationToken cancellationToken);

    Task<IEnumerable<(FileInfoData CollectFileInfo, string CollectUrl)>> GetRedirectedCollectItemsAsync(
        IEnumerable<(FileInfoData CollectFileInfo, string CollectUrl)> collectItems,
        string dataCollectionName,
        IEnumerable<KeyValuePair<string, string>> collectHeaders,
        int maxDegreeOfParallelism,
        IdentityServiceClientInfo identityServiceClientInfo,
        CancellationToken cancellationToken);
}
