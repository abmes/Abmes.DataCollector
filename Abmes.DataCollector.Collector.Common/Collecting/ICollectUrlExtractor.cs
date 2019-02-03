using System;
using System.Collections.Generic;
using System.Text;

namespace Abmes.DataCollector.Collector.Common.Collecting
{
    public interface ICollectUrlExtractor
    {
        string ExtractCollectUrl(string dataCollectionName, string collectFileIdentifier, string sourceUrl, IEnumerable<KeyValuePair<string, string>> headers);
    }
}
