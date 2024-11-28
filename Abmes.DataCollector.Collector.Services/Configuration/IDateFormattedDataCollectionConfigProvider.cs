using Abmes.DataCollector.Collector.Services.Ports.Configuration;

namespace Abmes.DataCollector.Collector.Services.Configuration;

public interface IDateFormattedDataCollectionConfigProvider
{
    DataCollectionConfig GetConfig(DataCollectionConfig config);
}
