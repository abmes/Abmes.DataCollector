using Abmes.DataCollector.Collector.Common.Misc;
using Abmes.DataCollector.Utils;
using Microsoft.Extensions.Logging;
using Polly;

namespace Abmes.DataCollector.Collector.Common.Collecting
{
    public class CollectUrlExtractor : ICollectUrlExtractor
    {
        private readonly IIdentityServiceHttpRequestConfigurator _identityServiceHttpRequestConfigurator;
        private readonly ILogger<CollectUrlExtractor> _logger;
        private readonly IHttpClientFactory _httpClientFactory;

        public CollectUrlExtractor(
            IIdentityServiceHttpRequestConfigurator identityServiceHttpRequestConfigurator,
            ILogger<CollectUrlExtractor> logger,
            IHttpClientFactory httpClientFactory)
        {
            _identityServiceHttpRequestConfigurator = identityServiceHttpRequestConfigurator;
            _logger = logger;
            _httpClientFactory = httpClientFactory;

        }

        public async Task<string> ExtractCollectUrlAsync(string dataCollectionName, string collectFileIdentifier, string sourceUrl, IEnumerable<KeyValuePair<string, string>> headers, string identityServiceAccessToken, CancellationToken cancellationToken)
        {
            var tryNo = 0;

            return
                await Policy
                    .Handle<Exception>()
                    .WaitAndRetryAsync(5, x => TimeSpan.FromSeconds(10))
                    .ExecuteAsync(
                        async (ct) =>
                        {
                            tryNo++;

                            if (tryNo > 1)
                            {
                                _logger.LogInformation($"Retrying ({tryNo-1}) get collect url for file '{collectFileIdentifier}' in data collection '{dataCollectionName}'");
                            }

                            using var httpClient = _httpClientFactory.CreateClient();

                            var collectUrlsJson = 
                                await httpClient.GetStringAsync(
                                    sourceUrl,
                                    HttpMethod.Get,
                                    headers: headers,
                                    accept: "application/json",
                                    requestConfiguratorTask: (request, ct) => Task.Run(() => _identityServiceHttpRequestConfigurator.Config(request, identityServiceAccessToken, ct), ct),
                                    cancellationToken: cancellationToken);

                            return HttpUtils.FixJsonResult(collectUrlsJson);
                        },
                        cancellationToken
                    );
        }
    }
}
