using Abmes.DataCollector.Collector.Common.Misc;
using Abmes.DataCollector.Utils;
using Microsoft.Extensions.Logging;
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
        private readonly ILogger<CollectUrlExtractor> _logger;

        public CollectUrlExtractor(
            IIdentityServiceHttpRequestConfigurator identityServiceHttpRequestConfigurator,
            ILogger<CollectUrlExtractor> logger)
        {
            _identityServiceHttpRequestConfigurator = identityServiceHttpRequestConfigurator;
            _logger = logger;
        }

        public async Task<string> ExtractCollectUrlAsync(string dataCollectionName, string collectFileIdentifier, string sourceUrl, IEnumerable<KeyValuePair<string, string>> headers, string identityServiceAccessToken, CancellationToken cancellationToken)
        {
            var tryNo = 0;

            return
                await Policy
                    .Handle<Exception>()
                    .WaitAndRetryAsync(5, x => TimeSpan.FromSeconds(10))
                    .ExecuteAsync(async (ct) =>
                        {
                            tryNo++;

                            if (tryNo > 1)
                            {
                                _logger.LogInformation($"Retrying ({tryNo-1}) get collect url for file '{collectFileIdentifier}' in data collection '{dataCollectionName}'");
                            }

                            var collectUrlsJson = 
                                    await HttpUtils.GetStringAsync(sourceUrl, HttpMethod.Get,
                                    headers: headers,
                                    accept: "application/json",
                                    requestConfiguratorTask: request => Task.Run(() => { _identityServiceHttpRequestConfigurator.Config(request, identityServiceAccessToken, cancellationToken); }),
                                    cancellationToken: cancellationToken);

                            return HttpUtils.FixJsonResult(collectUrlsJson);
                        },
                        cancellationToken
                    );
        }
    }
}
