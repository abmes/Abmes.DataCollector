namespace Abmes.DataCollector.Collector.Services.Configuration;

public interface IDataCollectionsFilterProvider
{
    Task<string> GetDataCollectionsFilterAsync(CancellationToken cancellationToken);
}
