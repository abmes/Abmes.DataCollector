using System;
using System.Collections.Generic;
using System.Text;

namespace Abmes.DataCollector.Collector.Common.Collecting
{
    public interface ICollectUrlsProvider
    {
        IEnumerable<string> GetCollectUrls(string dataCollectionName, string collectFileIdentifiersUrl, IEnumerable<KeyValuePair<string, string>> collectFileIdentifiersHeaders, string collectUrl, IEnumerable<KeyValuePair<string, string>> collectHeaders);
    }
}
