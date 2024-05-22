using Abmes.DataCollector.Collector.Services.Abstractions.AppConfig;
using Abmes.DataCollector.Collector.Services.Configuration;

namespace Abmes.DataCollector.Collector.App.Library.Configuration;

public class DataCollectionsFilterProvider(
    IBootstrapper bootstrapper) : IDataCollectionsFilterProvider
{
    public async Task<string> GetDataCollectionsFilterAsync(CancellationToken cancellationToken)
    {
        var args = Environment.GetCommandLineArgs();

        var result =
            !string.IsNullOrEmpty(bootstrapper.DataCollectionNames)
            ? bootstrapper.DataCollectionNames
            : args.Length > 2
                ? args[2]
                : string.Empty;

        return await Task.FromResult(result);
    }
}
