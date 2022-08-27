namespace Abmes.DataCollector.Collector.Common.Configuration;

public interface IDataCollectionsConfigProvider
{
    Task<IEnumerable<DataCollectionConfig>> GetDataCollectionsConfigAsync(string configSetName, CancellationToken cancellationToken);
}
