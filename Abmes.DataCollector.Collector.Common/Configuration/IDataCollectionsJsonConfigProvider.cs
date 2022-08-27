namespace Abmes.DataCollector.Collector.Common.Configuration
{
    public interface IDataCollectionsJsonConfigsProvider
    {
        IEnumerable<DataCollectionConfig> GetDataCollectionsConfig(string json);
    }
}
