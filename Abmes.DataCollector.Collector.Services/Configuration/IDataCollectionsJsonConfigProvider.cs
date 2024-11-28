using Abmes.DataCollector.Collector.Services.Ports.Configuration;

namespace Abmes.DataCollector.Collector.Services.Configuration;

public interface IDataCollectionsJsonConfigsProvider
{
    IEnumerable<DataCollectionConfig> GetDataCollectionsConfig(string json);
}
