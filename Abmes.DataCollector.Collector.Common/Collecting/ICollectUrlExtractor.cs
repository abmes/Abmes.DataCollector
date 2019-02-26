using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Abmes.DataCollector.Collector.Common.Collecting
{
    public interface ICollectUrlExtractor
    {
        Task<string> ExtractCollectUrlAsync(string dataCollectionName, string collectFileIdentifier, string sourceUrl, IEnumerable<KeyValuePair<string, string>> headers, CancellationToken cancellationToken);
    }
}
