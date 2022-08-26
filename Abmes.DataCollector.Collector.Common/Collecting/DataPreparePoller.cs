using System.Text.Json;
using Abmes.DataCollector.Collector.Common.Misc;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Abmes.DataCollector.Utils;

namespace Abmes.DataCollector.Collector.Common.Collecting
{
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
            var exportLogDataContent = await httpClient.GetStringAsync(pollUrl, pollHeaders, "application/json");

            var options = new JsonSerializerOptions() { PropertyNameCaseInsensitive = true };
            var result = JsonSerializer.Deserialize<DataPrepareResult>(exportLogDataContent, options);

            return result;
        }
    }
}
