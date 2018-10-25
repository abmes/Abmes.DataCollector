using Newtonsoft.Json;
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
        public async Task<DataPrepareResult> GetDataPrepareResultAsync(string pollUrl, IEnumerable<KeyValuePair<string, string>> pollHeaders, CancellationToken cancellationToken)
        {
            var exportLogDataContent = await HttpUtils.GetString(pollUrl, pollHeaders, "application/json");

            dynamic exportLogData = JsonConvert.DeserializeObject(exportLogDataContent);

            return new DataPrepareResult { Finished = exportLogData.finished, HasErrors = exportLogData.hasErrors };
        }
    }
}
