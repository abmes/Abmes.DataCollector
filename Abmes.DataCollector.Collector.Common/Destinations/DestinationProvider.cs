using Abmes.DataCollector.Collector.Common.Configuration;

namespace Abmes.DataCollector.Collector.Common.Destinations;

public class DestinationProvider(
    IDestinationsConfigProvider destinationsConfigProvider,
    IConfigSetNameProvider configSetNameProvider,
    IDestinationResolverProvider destinationResolverProvider) : IDestinationProvider
{
    public async Task<IDestination> GetDestinationAsync(string destinationId, CancellationToken cancellationToken)
    {
        var destinationConfig = await GetDestinationConfigAsync(destinationId, cancellationToken);
        var resolver = destinationResolverProvider.GetResolver(destinationConfig);
        return resolver.GetDestination(destinationConfig);
    }

    private async Task<DestinationConfig> GetDestinationConfigAsync(string destinationId, CancellationToken cancellationToken)
    {
        var configSetName = configSetNameProvider.GetConfigSetName();
        var destinationsConfig = await destinationsConfigProvider.GetDestinationsConfigAsync(configSetName, cancellationToken);
        var destinationConfig = destinationsConfig.Where(x => destinationId.Equals(x.DestinationId, StringComparison.CurrentCultureIgnoreCase)).SingleOrDefault();

        if (destinationConfig is null)
        {
            throw new Exception($"Destination with id '{destinationId}' not found");
        }

        return destinationConfig;
    }
}
