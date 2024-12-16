using Abmes.DataCollector.Collector.Data.Common.Identity;
using Abmes.DataCollector.Collector.Services.Ports.Destinations;
using Abmes.DataCollector.Collector.Services.Ports.Identity;
using Abmes.Utils;
using Abmes.Utils.Net;

namespace Abmes.DataCollector.Collector.Data.Console.Destinations;

public class ConsoleDestination(
    DestinationConfig destinationConfig,
    IIdentityServiceHttpRequestConfigurator identityServiceHttpRequestConfigurator,
    IHttpClientFactory httpClientFactory) : IConsoleDestination
{
    public DestinationConfig DestinationConfig => destinationConfig;

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
        using var httpClient = httpClientFactory.CreateClient();
        var content =
            await httpClient.GetStringAsync(
                collectUrl,
                HttpMethod.Get,
                null,
                null,
                collectHeaders,
                null,
                timeout,
                (request, ct) => identityServiceHttpRequestConfigurator.ConfigAsync(request, collectIdentityServiceClientInfo, ct),
                cancellationToken: cancellationToken);

        System.Console.WriteLine(content);
    }

    public async Task<IEnumerable<string>> GetDataCollectionFileNamesAsync(string dataCollectionName, CancellationToken cancellationToken)
    {
        await Task.CompletedTask;
        throw new NotImplementedException();
    }

    public async Task GarbageCollectDataCollectionFileAsync(string dataCollectionName, string fileName, CancellationToken cancellationToken)
    {
        await Task.CompletedTask;
        throw new NotImplementedException();
    }

    public bool CanGarbageCollect()
    {
        return false;
    }

    public Task<bool> AcceptsFileAsync(string dataCollectionName, string name, long? size, string? md5, CancellationToken cancellationToken)
    {
        return Task.FromResult(true);
    }

    public async Task PutFileAsync(string dataCollectionName, string fileName, Stream content, CancellationToken cancellationToken)
    {
        await foreach (var line in content.ReadAllLinesAsync(cancellationToken))
        {
            System.Console.WriteLine(line);
        }
    }
}
