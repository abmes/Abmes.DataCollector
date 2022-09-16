﻿using System.Text.Json;
using Abmes.DataCollector.Utils;

namespace Abmes.DataCollector.Collector.Common.Collecting;

public class DataPreparePoller : IDataPreparePoller
{
    private readonly IHttpClientFactory _httpClientFactory;

    public DataPreparePoller(IHttpClientFactory httpClientFactory)
    {
        _httpClientFactory = httpClientFactory;
    }

    public async Task<DataPrepareResult> GetDataPrepareResultAsync(string pollUrl, IEnumerable<KeyValuePair<string, string>> pollHeaders, CancellationToken cancellationToken)
    {
        using var httpClient = _httpClientFactory.CreateClient();
        var exportLogDataContent = await httpClient.GetStringAsync(pollUrl, pollHeaders, "application/json", cancellationToken: cancellationToken);

        var options = new JsonSerializerOptions() { PropertyNameCaseInsensitive = true };
        var result = JsonSerializer.Deserialize<DataPrepareResult>(exportLogDataContent, options) ?? new DataPrepareResult(Finished: false, HasErrors: false);

        return result;
    }
}
