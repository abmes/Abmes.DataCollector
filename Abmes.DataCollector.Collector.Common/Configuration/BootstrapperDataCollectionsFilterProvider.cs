namespace Abmes.DataCollector.Collector.Common.Configuration;

public class BootstrapperDataCollectionsFilterProvider : IDataCollectionsFilterProvider
{
    private readonly IBootstrapper _bootstrapper;

    public BootstrapperDataCollectionsFilterProvider(IBootstrapper bootstrapper)
    {
        _bootstrapper = bootstrapper;
    }

    public async Task<string> GetDataCollectionsFilterAsync(CancellationToken cancellationToken)
    {
        return await Task.FromResult(_bootstrapper.DataCollectionNames);
    }
}
