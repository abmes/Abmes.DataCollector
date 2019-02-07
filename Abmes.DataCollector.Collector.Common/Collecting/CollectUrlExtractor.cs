using Abmes.DataCollector.Utils;
using Newtonsoft.Json;
using Polly;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;

namespace Abmes.DataCollector.Collector.Common.Collecting
{
    public class CollectUrlExtractor : ICollectUrlExtractor
    {
        public string ExtractCollectUrl(string dataCollectionName, string collectFileIdentifier, string sourceUrl, IEnumerable<KeyValuePair<string, string>> headers, CancellationToken cancellationToken)
        {
            return
                Policy
                    .Handle<Exception>()
                    .WaitAndRetry(1, x => TimeSpan.FromSeconds(5))
                    .Execute((ct) =>
                        {
                            var collectUrlsJson = HttpUtils.GetString(sourceUrl, headers, "application/json").Result;
                            return TrimPseudoNewLine(collectUrlsJson.Trim('"').Trim());
                        },
                        cancellationToken
                    );
        }

        private string TrimPseudoNewLine(string s)
        {
            while ((!string.IsNullOrEmpty(s)) && (s.EndsWith(@"\n") || s.EndsWith(@"\r")))
            {
                s = s.Remove(s.Length - 2);
            }

            return s;
        }
    }
}
