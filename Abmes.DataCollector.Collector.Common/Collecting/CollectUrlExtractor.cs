using Abmes.DataCollector.Collector.Common.Misc;
using Abmes.DataCollector.Utils;
using Newtonsoft.Json;
using Polly;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Abmes.DataCollector.Collector.Common.Collecting
{
    public class CollectUrlExtractor : ICollectUrlExtractor
    {
        private readonly IIdentityServiceHttpRequestConfigurator _identityServiceHttpRequestConfigurator;

        public CollectUrlExtractor(
            IIdentityServiceHttpRequestConfigurator identityServiceHttpRequestConfigurator)
        {
            _identityServiceHttpRequestConfigurator = identityServiceHttpRequestConfigurator;
        }

        public async Task<string> ExtractCollectUrlAsync(string dataCollectionName, string collectFileIdentifier, string sourceUrl, IEnumerable<KeyValuePair<string, string>> headers, string identityServiceAccessToken, CancellationToken cancellationToken)
        {
            return
                await Policy
                    .Handle<Exception>()
                    .WaitAndRetryAsync(1, x => TimeSpan.FromSeconds(5))
                    .ExecuteAsync(async (ct) =>
                        {
                            var collectUrlsJson = 
                                    await HttpUtils.GetStringAsync(sourceUrl, HttpMethod.Get,
                                    headers: headers,
                                    accept: "application/json",
                                    requestConfiguratorTask: request => Task.Run(() => { _identityServiceHttpRequestConfigurator.Config(request, identityServiceAccessToken, cancellationToken); }),
                                    cancellationToken: cancellationToken);

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
