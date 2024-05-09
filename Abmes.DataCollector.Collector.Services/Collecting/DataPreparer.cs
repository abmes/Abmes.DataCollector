﻿using Abmes.DataCollector.Collector.Services.Configuration;
using Abmes.DataCollector.Collector.Services.Misc;
using Abmes.DataCollector.Utils;
using Abmes.DataCollector.Utils.Net;

namespace Abmes.DataCollector.Collector.Services.Collecting;

public class DataPreparer(
    IDataPreparePoller dataPreparePoller,
    IDelay delay,
    IHttpClientFactory httpClientFactory) : IDataPreparer
{
    public async Task<bool> PrepareDataAsync(DataCollectionConfig dataCollectionConfig, CancellationToken cancellationToken)
    {
        if (string.IsNullOrEmpty(dataCollectionConfig.PrepareUrl))
        {
            return false;
        }

        await PrepareCollectAsync(dataCollectionConfig.PrepareUrl, dataCollectionConfig.PrepareHeaders, Ensure.NotNullOrEmpty(dataCollectionConfig.PrepareHttpMethod), cancellationToken);

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
        await delay.DelayAsync(dataCollectionConfig.PrepareDuration ?? default, $"Data {dataCollectionConfig.DataCollectionName} prepare to finish", cancellationToken);
    }

    private async Task PrepareCollectAsync(string prepareUrl, IEnumerable<KeyValuePair<string, string>> prepareHeaders, string prepareHttpMethod, CancellationToken cancellationToken)
    {
        var urls = prepareUrl.Split('|').ToList();
        var preliminaryUrls = urls.SkipLast(1);
        var lastUrl = urls.Last();
        using var httpClient = httpClientFactory.CreateClient();
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
                prepareResult = await dataPreparePoller.GetDataPrepareResultAsync(pollUrl, pollHeaders, cancellationToken);
            }
            catch
            {
                // try again after pollInterval
                prepareResult = new DataPrepareResult(Finished: false, HasErrors: true);
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
