namespace Abmes.DataCollector.Collector.Common.Configuration
{
    public interface IMergedDataCollectionConfigProvider
    {
        DataCollectionConfig GetConfig(DataCollectionConfig config, DataCollectionConfig template);
    }
}
