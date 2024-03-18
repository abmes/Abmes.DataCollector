namespace Abmes.DataCollector.Collector.Common.Configuration;

public class BootstrapperDataCollectionsFilterProvider(
    IBootstrapper bootstrapper) : IDataCollectionsFilterProvider
{
    public async Task<string> GetDataCollectionsFilterAsync(CancellationToken cancellationToken)
    {
        return await Task.FromResult(bootstrapper.DataCollectionNames);
    }
}
