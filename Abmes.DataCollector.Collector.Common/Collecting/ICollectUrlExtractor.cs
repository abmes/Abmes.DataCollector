using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;

namespace Abmes.DataCollector.Collector.Common.Collecting
{
    public interface ICollectUrlExtractor
    {
        string ExtractCollectUrl(string dataCollectionName, string collectFileIdentifier, string sourceUrl, IEnumerable<KeyValuePair<string, string>> headers, CancellationToken cancellationToken);
    }
}
