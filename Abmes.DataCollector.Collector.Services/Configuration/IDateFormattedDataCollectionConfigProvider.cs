namespace Abmes.DataCollector.Collector.Services.Configuration;

public interface IDateFormattedDataCollectionConfigProvider
{
    DataCollectionConfig GetConfig(DataCollectionConfig config);
}
