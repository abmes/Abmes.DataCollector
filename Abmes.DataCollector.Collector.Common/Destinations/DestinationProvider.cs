using Abmes.DataCollector.Collector.Common.Configuration;

namespace Abmes.DataCollector.Collector.Common.Destinations;

public class DestinationProvider : IDestinationProvider
{
    private readonly IDestinationsConfigProvider _destinationsConfigProvider;
    private readonly IConfigSetNameProvider _configSetNameProvider;
    private readonly IDestinationResolverProvider _destinationResolverProvider;

    public DestinationProvider(
        IDestinationsConfigProvider destinationsConfigProvider,
        IConfigSetNameProvider configSetNameProvider,
        IDestinationResolverProvider destinationResolverProvider)
    {
        _destinationsConfigProvider = destinationsConfigProvider;
        _configSetNameProvider = configSetNameProvider;
        _destinationResolverProvider = destinationResolverProvider;
    }

    public async Task<IDestination> GetDestinationAsync(string destinationId, CancellationToken cancellationToken)
    {
        var destinationConfig = await GetDestinationConfigAsync(destinationId, cancellationToken);
        var resolver = _destinationResolverProvider.GetResolver(destinationConfig);
        return resolver.GetDestination(destinationConfig);
    }

    private async Task<DestinationConfig> GetDestinationConfigAsync(string destinationId, CancellationToken cancellationToken)
    {
        var configSetName = _configSetNameProvider.GetConfigSetName();
        var destinationsConfig = await _destinationsConfigProvider.GetDestinationsConfigAsync(configSetName, cancellationToken);
        var destinationConfig = destinationsConfig.Where(x => destinationId.Equals(x.DestinationId, StringComparison.CurrentCultureIgnoreCase)).SingleOrDefault();

        if (destinationConfig is null)
        {
            throw new Exception($"Destination with id '{destinationId}' not found");
        }

        return destinationConfig;
    }
}
