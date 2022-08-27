using Abmes.DataCollector.Collector.Common.Configuration;

namespace Abmes.DataCollector.Collector.ConsoleApp.Configuration;

public class DataCollectionsFilterProvider : IDataCollectionsFilterProvider
{
    private readonly IBootstrapper _bootstrapper;

    public DataCollectionsFilterProvider(IBootstrapper bootstrapper)
    {
        _bootstrapper = bootstrapper;
    }

    public async Task<string> GetDataCollectionsFilterAsync(CancellationToken cancellationToken)
    {
        var args = Environment.GetCommandLineArgs();

        var result = _bootstrapper.DataCollectionNames ?? ((args.Length > 2) ? args[2] : null);

        return await Task.FromResult(result);
    }
}
