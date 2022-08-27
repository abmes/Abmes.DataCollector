namespace Abmes.DataCollector.Collector.Common.Configuration;

public interface IDataCollectionsFilterProvider
{
    Task<string> GetDataCollectionsFilterAsync(CancellationToken cancellationToken);
}
