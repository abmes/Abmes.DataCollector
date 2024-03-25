namespace Abmes.DataCollector.Collector.Services.Configuration;

public interface IDataCollectionsJsonConfigsProvider
{
    IEnumerable<DataCollectionConfig> GetDataCollectionsConfig(string json);
}
