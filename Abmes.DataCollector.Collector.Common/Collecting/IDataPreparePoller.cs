﻿namespace Abmes.DataCollector.Collector.Common.Collecting
{
    public interface IDataPreparePoller
    {
        Task<DataPrepareResult> GetDataPrepareResultAsync(string pollUrl, IEnumerable<KeyValuePair<string, string>> pollHeaders, CancellationToken cancellationToken);
    }
}
