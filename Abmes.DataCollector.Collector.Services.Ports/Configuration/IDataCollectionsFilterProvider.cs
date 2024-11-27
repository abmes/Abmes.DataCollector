namespace Abmes.DataCollector.Collector.Services.Ports.Configuration;

public interface IDataCollectionsFilterProvider
{
    Task<string> GetDataCollectionsFilterAsync(CancellationToken cancellationToken);
}
