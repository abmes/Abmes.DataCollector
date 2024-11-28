using Abmes.DataCollector.Collector.Services.Ports.Configuration;

namespace Abmes.DataCollector.Collector.Services.Configuration;

public interface IMergedDataCollectionConfigProvider
{
    DataCollectionConfig GetConfig(DataCollectionConfig config, DataCollectionConfig template);
}
