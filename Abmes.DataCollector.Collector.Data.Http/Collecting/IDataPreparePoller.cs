namespace Abmes.DataCollector.Collector.Data.Http.Collecting;

public interface IDataPreparePoller
{
    Task<DataPrepareResult> GetDataPrepareResultAsync(string pollUrl, IEnumerable<KeyValuePair<string, string>> pollHeaders, CancellationToken cancellationToken);
}
