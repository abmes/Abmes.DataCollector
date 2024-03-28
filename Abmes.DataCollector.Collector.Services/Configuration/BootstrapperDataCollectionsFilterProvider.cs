using Abmes.DataCollector.Collector.Services.Abstractions.Configuration;

namespace Abmes.DataCollector.Collector.Services.Configuration;

public class BootstrapperDataCollectionsFilterProvider(  // todo: this should be in web.library
    IBootstrapper bootstrapper) : IDataCollectionsFilterProvider
{
    public async Task<string> GetDataCollectionsFilterAsync(CancellationToken cancellationToken)
    {
        return await Task.FromResult(bootstrapper.DataCollectionNames);
    }
}
