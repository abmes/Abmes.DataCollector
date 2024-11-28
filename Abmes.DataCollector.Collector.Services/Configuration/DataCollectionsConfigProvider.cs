using Abmes.DataCollector.Collector.Services.Ports.Configuration;
using Abmes.DataCollector.Common.Services.Ports.Configuration;

namespace Abmes.DataCollector.Collector.Services.Configuration;

public class DataCollectionsConfigProvider(
    IDataCollectionsJsonConfigsProvider dataCollectJsonConfigsProvider,
    IConfigProvider configProvider) : IDataCollectionsConfigProvider
{
    private const string DataCollectionsConfigFileName = "DataCollectionsConfig.json";

    public async Task<IEnumerable<DataCollectionConfig>> GetDataCollectionsConfigAsync(string configSetName, CancellationToken cancellationToken)
    {
        var configName = (configSetName + "/" + DataCollectionsConfigFileName).TrimStart('/');
        var json = await configProvider.GetConfigContentAsync(configName, cancellationToken);
        return dataCollectJsonConfigsProvider.GetDataCollectionsConfig(json);
    }
}
