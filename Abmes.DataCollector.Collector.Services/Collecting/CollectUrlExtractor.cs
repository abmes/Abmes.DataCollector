using Abmes.DataCollector.Collector.Services.Misc;
using Abmes.DataCollector.Utils;
using Microsoft.Extensions.Logging;

namespace Abmes.DataCollector.Collector.Services.Collecting;

public class CollectUrlExtractor(
    IIdentityServiceHttpRequestConfigurator identityServiceHttpRequestConfigurator,
    IAsyncExecutionStrategy<CollectUrlExtractor> executionStrategy,
    ILogger<CollectUrlExtractor> logger,
    IHttpClientFactory httpClientFactory) : ICollectUrlExtractor
{
    public async Task<string> ExtractCollectUrlAsync(string dataCollectionName, string? collectFileIdentifier, string sourceUrl, IEnumerable<KeyValuePair<string, string>> headers, string? identityServiceAccessToken, CancellationToken cancellationToken)
    {
        return await
            executionStrategy.ExecuteAsync(
                async (ct) =>
                {
                    logger.LogInformation("Trying to get collect url for file '{collectFileIdentifier}' in data collection '{dataCollectionName}'", collectFileIdentifier, dataCollectionName);

                    using var httpClient = httpClientFactory.CreateClient();

                    var collectUrlsJson = 
                        await httpClient.GetStringAsync(
                            sourceUrl,
                            HttpMethod.Get,
                            headers: headers,
                            accept: "application/json",
                            requestConfiguratorTask: (request, ct) => Task.Run(() => identityServiceHttpRequestConfigurator.Config(request, identityServiceAccessToken, ct), ct),
                            cancellationToken: cancellationToken);

                    return HttpUtils.FixJsonResult(collectUrlsJson);
                },
                cancellationToken
            );
    }
}
