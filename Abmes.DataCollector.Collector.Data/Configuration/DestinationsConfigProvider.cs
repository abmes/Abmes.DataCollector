using Abmes.DataCollector.Common.Data.Configuration;

namespace Abmes.DataCollector.Collector.Data.Configuration;

public class DestinationsConfigProvider(
    IDestinationsJsonConfigProvider destinationsJsonConfigProvider,
    IConfigProvider configProvider) : IDestinationsConfigProvider
{
    private const string DestinationsConfigFileName = "DestinationsConfig.json";

    public async Task<IEnumerable<DestinationConfig>> GetDestinationsConfigAsync(string configSetName, CancellationToken cancellationToken)
    {
        var configBlobName = (configSetName + "/" + DestinationsConfigFileName).TrimStart('/');
        var json = await configProvider.GetConfigContentAsync(configBlobName, cancellationToken);
        return destinationsJsonConfigProvider.GetDestinationsConfig(json);
    }
}
