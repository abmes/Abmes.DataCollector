namespace Abmes.DataCollector.Collector.Common.Configuration;

public interface IDateFormattedDataCollectionConfigProvider
{
    DataCollectionConfig GetConfig(DataCollectionConfig config);
}
