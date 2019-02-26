using Abmes.DataCollector.Utils;
using Newtonsoft.Json;
using Polly;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Abmes.DataCollector.Collector.Common.Collecting
{
    public class CollectUrlExtractor : ICollectUrlExtractor
    {
        public async Task<string> ExtractCollectUrlAsync(string dataCollectionName, string collectFileIdentifier, string sourceUrl, IEnumerable<KeyValuePair<string, string>> headers, CancellationToken cancellationToken)
        {
            return
                await Policy
                    .Handle<Exception>()
                    .WaitAndRetryAsync(1, x => TimeSpan.FromSeconds(5))
                    .ExecuteAsync(async (ct) =>
                        {
                            var collectUrlsJson = await HttpUtils.GetString(sourceUrl, headers, "application/json");
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
