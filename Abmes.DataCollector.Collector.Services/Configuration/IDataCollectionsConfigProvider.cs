using Abmes.DataCollector.Collector.Services.Ports.Configuration;

namespace Abmes.DataCollector.Collector.Services.Configuration;

public interface IDataCollectionsConfigProvider
{
    Task<IEnumerable<DataCollectionConfig>> GetDataCollectionsConfigAsync(string configSetName, CancellationToken cancellationToken);
}
