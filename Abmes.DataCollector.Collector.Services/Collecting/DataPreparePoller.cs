using Abmes.DataCollector.Utils.Net;
using System.Text.Json;

namespace Abmes.DataCollector.Collector.Services.Collecting;

public class DataPreparePoller(
    IHttpClientFactory httpClientFactory) : IDataPreparePoller
{
    private static readonly JsonSerializerOptions _jsonSerializerOptions = new() { PropertyNameCaseInsensitive = true };

    public async Task<DataPrepareResult> GetDataPrepareResultAsync(string pollUrl, IEnumerable<KeyValuePair<string, string>> pollHeaders, CancellationToken cancellationToken)
    {
        using var httpClient = httpClientFactory.CreateClient();
        var exportLogDataContent = await httpClient.GetStringAsync(pollUrl, pollHeaders, "application/json", cancellationToken: cancellationToken);

        var result = JsonSerializer.Deserialize<DataPrepareResult>(exportLogDataContent, _jsonSerializerOptions) ?? new DataPrepareResult(Finished: false, HasErrors: false);

        return result;
    }
}
