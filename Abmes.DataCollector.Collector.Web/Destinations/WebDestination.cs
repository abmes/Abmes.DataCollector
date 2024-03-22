using Abmes.DataCollector.Collector.Common.Configuration;
using Abmes.DataCollector.Collector.Common.Misc;
using Abmes.DataCollector.Utils;
using System.Text.Json;

namespace Abmes.DataCollector.Collector.Web.Destinations;

public class WebDestination(
    DestinationConfig destinationConfig,
    IIdentityServiceHttpRequestConfigurator identityServiceHttpRequestConfigurator,
    IHttpClientFactory httpClientFactory) : IWebDestination
{
    public DestinationConfig DestinationConfig => destinationConfig;

    private static readonly JsonSerializerOptions _jsonSerializerOptions = new() { PropertyNameCaseInsensitive = true };

    public async Task CollectAsync(
        string collectUrl,
        IEnumerable<KeyValuePair<string, string>> collectHeaders,
        IdentityServiceClientInfo? collectIdentityServiceClientInfo,
        string dataCollectionName,
        string fileName,
        TimeSpan timeout,
        bool finishWait,
        CancellationToken cancellationToken)
    {
        var endpointUrl = GetEndpointUrl(DestinationConfig.CollectPostEndpoint, dataCollectionName, fileName);

        if (string.IsNullOrEmpty(endpointUrl))
        {
            return;
        }

        using var httpClient = httpClientFactory.CreateClient();
        using var _ = await httpClient.SendAsync(
            endpointUrl,
            HttpMethod.Post,
            collectUrl,
            null,
            null,
            collectHeaders,
            null,
            timeout,
            (request, ct) => identityServiceHttpRequestConfigurator.ConfigAsync(request, DestinationConfig.IdentityServiceClientInfo, ct),
            cancellationToken: cancellationToken);
    }

    public async Task<IEnumerable<string>> GetDataCollectionFileNamesAsync(string dataCollectionName, CancellationToken cancellationToken)
    {
        var endpointUrl = GetEndpointUrl(DestinationConfig.FileNamesGetEndpoint, dataCollectionName, null);
        ArgumentNullException.ThrowIfNull(endpointUrl);

        using var httpClient = httpClientFactory.CreateClient();
        var json =
            await httpClient.GetStringAsync(
                endpointUrl,
                HttpMethod.Get,
                accept: "application/json",
                requestConfiguratorTask: (request, ct) => identityServiceHttpRequestConfigurator.ConfigAsync(request, DestinationConfig.IdentityServiceClientInfo, ct),
                cancellationToken: cancellationToken);

        return JsonSerializer.Deserialize<IEnumerable<string>>(json, _jsonSerializerOptions) ?? [];
    }

    public async Task GarbageCollectDataCollectionFileAsync(string dataCollectionName, string fileName, CancellationToken cancellationToken)
    {
        var endpointUrl = GetEndpointUrl(DestinationConfig.GarbageCollectFilePostEndpoint, dataCollectionName, fileName);
        ArgumentNullException.ThrowIfNull(endpointUrl);

        using var httpClient = httpClientFactory.CreateClient();
        using var _ = await httpClient.SendAsync(
            endpointUrl,
            HttpMethod.Post,
            requestConfiguratorTask: (request, ct) => identityServiceHttpRequestConfigurator.ConfigAsync(request, DestinationConfig.IdentityServiceClientInfo, ct),
            cancellationToken: cancellationToken);
    }

    private string? GetEndpointUrl(string? endpoint, string dataCollectionName, string? fileName)
    {
        if (string.IsNullOrEmpty(DestinationConfig.Root) || string.IsNullOrEmpty(endpoint))
        {
            return null;
        }

        var result = DestinationConfig.Root.TrimEnd('/') + "/" + endpoint.TrimStart('/');

        result = result.Replace("[DataCollectionName]", dataCollectionName);

        if (!string.IsNullOrEmpty(fileName))
        {
            result = result.Replace("[FileName]", fileName);
        }

        return result;
    }

    public bool CanGarbageCollect()
    {
        return 
            (!string.IsNullOrEmpty(DestinationConfig.Root)) && 
            (!string.IsNullOrEmpty(DestinationConfig.FileNamesGetEndpoint)) &&
            (!string.IsNullOrEmpty(DestinationConfig.GarbageCollectFilePostEndpoint));
    }

    public Task<bool> AcceptsFileAsync(string dataCollectionName, string name, long? size, string? md5, CancellationToken cancellationToken)
    {
        return Task.FromResult(true);
    }

    public async Task PutFileAsync(string dataCollectionName, string fileName, Stream content, CancellationToken cancellationToken)
    {
        var endpointUrl = GetEndpointUrl(DestinationConfig.CollectPostEndpoint, dataCollectionName, fileName);

        if (string.IsNullOrEmpty(endpointUrl))
        {
            return;
        }

        using var httpClient = httpClientFactory.CreateClient();
        using var _ = await httpClient.SendAsync(
            endpointUrl,
            HttpMethod.Post,
            content: content,
            requestConfiguratorTask: (request, ct) => identityServiceHttpRequestConfigurator.ConfigAsync(request, DestinationConfig.IdentityServiceClientInfo, ct),
            cancellationToken: cancellationToken);
    }
}
