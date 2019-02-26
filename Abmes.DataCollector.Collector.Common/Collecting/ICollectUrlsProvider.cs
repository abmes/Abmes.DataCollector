using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Abmes.DataCollector.Collector.Common.Collecting
{
    public interface ICollectUrlsProvider
    {
        Task<IEnumerable<string>> GetCollectUrlsAsync(string dataCollectionName, string collectFileIdentifiersUrl, IEnumerable<KeyValuePair<string, string>> collectFileIdentifiersHeaders, string collectUrl, IEnumerable<KeyValuePair<string, string>> collectHeaders, int maxDegreeOfParallelism, CancellationToken cancellationToken);
    }
}
