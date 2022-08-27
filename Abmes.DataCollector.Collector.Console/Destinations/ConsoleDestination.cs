using Abmes.DataCollector.Collector.Common.Configuration;
using Abmes.DataCollector.Collector.Common.Misc;
using Abmes.DataCollector.Utils;

namespace Abmes.DataCollector.Collector.Console.Destinations;

public class ConsoleDestination : IConsoleDestination
{
    public DestinationConfig DestinationConfig { get; }

    private readonly IIdentityServiceHttpRequestConfigurator _identityServiceHttpRequestConfigurator;
    private readonly IHttpClientFactory _httpClientFactory;

    public ConsoleDestination(
        DestinationConfig destinationConfig,
        IIdentityServiceHttpRequestConfigurator identityServiceHttpRequestConfigurator,
        IHttpClientFactory httpClientFactory)
    {
        DestinationConfig = destinationConfig;
        _identityServiceHttpRequestConfigurator = identityServiceHttpRequestConfigurator;
        _httpClientFactory = httpClientFactory;
    }

    public async Task CollectAsync(string collectUrl, IEnumerable<KeyValuePair<string, string>> collectHeaders, IIdentityServiceClientInfo collectIdentityServiceClientInfo, string dataCollectionName, string fileName, TimeSpan timeout, bool finishWait, int tryNo, CancellationToken cancellationToken)
    {
        using var httpClient = _httpClientFactory.CreateClient();
        var content =
            await httpClient.GetStringAsync(
                collectUrl,
                HttpMethod.Get,
                null,
                null,
                collectHeaders,
                null,
                timeout,
                (request, ct) => _identityServiceHttpRequestConfigurator.ConfigAsync(request, collectIdentityServiceClientInfo, ct),
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

    public Task<bool> AcceptsFileAsync(string dataCollectionName, string name, long? size, string md5, CancellationToken cancellationToken)
    {
        return Task.FromResult(true);
    }

    public async Task PutFileAsync(string dataCollectionName, string fileName, Stream content, CancellationToken cancellationToken)
    {
        using var reader = new StreamReader(content);
        while (true)
        {
            var line = await reader.ReadLineAsync();

            if (line == null)
            {
                break;
            }

            System.Console.WriteLine(line);
        }
    }
}
