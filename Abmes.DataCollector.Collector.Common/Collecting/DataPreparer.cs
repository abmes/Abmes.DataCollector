using Abmes.DataCollector.Collector.Common.Misc;
using Abmes.DataCollector.Utils;
using Abmes.DataCollector.Collector.Common.Configuration;

namespace Abmes.DataCollector.Collector.Common.Collecting
{
    public class DataPreparer : IDataPreparer
    {
        private readonly IDataPreparePoller _dataPreparePoller;
        private readonly IDelay _delay;
        private readonly IHttpClientFactory _httpClientFactory;

        public DataPreparer(IDataPreparePoller DataPreparePoller, IDelay delay, IHttpClientFactory httpClientFactory)
        {
            _dataPreparePoller = DataPreparePoller;
            _delay = delay;
            _httpClientFactory = httpClientFactory;
        }

        public async Task<bool> PrepareDataAsync(DataCollectionConfig dataCollectionConfig, CancellationToken cancellationToken)
        {
            if (string.IsNullOrEmpty(dataCollectionConfig.PrepareUrl))
            {
                return false;
            }

            await PrepareCollectAsync(dataCollectionConfig.PrepareUrl, dataCollectionConfig.PrepareHeaders, dataCollectionConfig.PrepareHttpMethod, cancellationToken);

            if (!string.IsNullOrEmpty(dataCollectionConfig.PrepareFinishedPollUrl))
            {
                await WaitPrepareToFinishAsync(dataCollectionConfig.PrepareFinishedPollUrl, dataCollectionConfig.PrepareFinishedPollHeaders, dataCollectionConfig.PrepareFinishedPollInterval, dataCollectionConfig.PrepareDuration, cancellationToken);
            }
            else
            {
                await WaitPrepareDurationAsync(dataCollectionConfig, cancellationToken);
            }

            return true;
        }

        private async Task WaitPrepareDurationAsync(DataCollectionConfig dataCollectionConfig, CancellationToken cancellationToken)
        {
            await _delay.DelayAsync(dataCollectionConfig.PrepareDuration ?? default, $"Data {dataCollectionConfig.DataCollectionName} prepare to finish", cancellationToken);
        }

        private async Task PrepareCollectAsync(string prepareUrl, IEnumerable<KeyValuePair<string, string>> prepareHeaders, string prepareHttpMethod, CancellationToken cancellationToken)
        {
            var urls = prepareUrl.Split('|').ToList();
            var preliminaryUrls = urls.SkipLast(1);
            var lastUrl = urls.Last();
            using var httpClient = _httpClientFactory.CreateClient();
            await httpClient.SendManyAsync(preliminaryUrls, new HttpMethod(prepareHttpMethod), prepareHeaders, cancellationToken);
            using var _ = await httpClient.SendAsync(lastUrl, new HttpMethod(prepareHttpMethod), headers: prepareHeaders, timeout: TimeSpan.FromMinutes(5), cancellationToken: cancellationToken);
        }

        private async Task WaitPrepareToFinishAsync(string pollUrl, IEnumerable<KeyValuePair<string, string>> pollHeaders, TimeSpan? pollInterval, TimeSpan? prepareDuration, CancellationToken cancellationToken)
        {
            if (!pollInterval.HasValue)
            {
                return;
            }

            var startTime = DateTimeOffset.UtcNow;
            DataPrepareResult prepareResult;

            while (true)
            {
                await Task.Delay(pollInterval.Value, cancellationToken);

                try
                {
                    prepareResult = await _dataPreparePoller.GetDataPrepareResultAsync(pollUrl, pollHeaders, cancellationToken);
                }
                catch
                {
                    // try again after pollInterval
                    prepareResult = new DataPrepareResult { Finished = false, HasErrors = true };
                }

                if ((!prepareResult.Finished) &&
                    ((prepareDuration ?? default).TotalMilliseconds > 0) && 
                    (DateTimeOffset.UtcNow.Subtract(startTime) > prepareDuration))
                {
                    throw new Exception($"Prepare timed out. ({prepareDuration.Value.TotalSeconds} seconds)");
                }

                if (prepareResult.Finished)
                {
                    if (prepareResult.HasErrors)
                    {
                        throw new Exception("Prepare finished with errors");
                    }

                    return;
                }
            }
        }
    }
}
