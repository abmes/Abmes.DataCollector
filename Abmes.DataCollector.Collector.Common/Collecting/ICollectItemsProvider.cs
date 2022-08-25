using Abmes.DataCollector.Collector.Common.Configuration;
using Abmes.DataCollector.Common.Storage;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Abmes.DataCollector.Collector.Common.Collecting
{
    public interface ICollectItemsProvider
    {
        IEnumerable<(IFileInfoData CollectFileInfo, string CollectUrl)> GetCollectItems(string dataCollectionName, string collectFileIdentifiersUrl, IEnumerable<KeyValuePair<string, string>> collectFileIdentifiersHeaders, string collectUrl, IEnumerable<KeyValuePair<string, string>> collectHeaders, IIdentityServiceClientInfo identityServiceClientInfo, CancellationToken cancellationToken);
        Task<IEnumerable<(IFileInfoData CollectFileInfo, string CollectUrl)>> GetRedirectedCollectItemsAsync(IEnumerable<(IFileInfoData CollectFileInfo, string CollectUrl)> collectItems, string dataCollectionName, IEnumerable<KeyValuePair<string, string>> collectHeaders, int maxDegreeOfParallelism, IIdentityServiceClientInfo identityServiceClientInfo, CancellationToken cancellationToken);
    }
}
