using Abmes.DataCollector.Collector.Common.Configuration;
using Abmes.DataCollector.Collector.Services.Configuration;

namespace Abmes.DataCollector.Collector.Services.Collecting;

public class MainCollector(
    IConfigSetNameProvider configSetNameProvider,
    IDataCollectionsConfigProvider dataCollectionsConfigProvider,
    IDataCollector dataCollector,
    ICollectorModeProvider collectorModeProvider) : IMainCollector
{
    public async Task<IEnumerable<string>> CollectAsync(CancellationToken cancellationToken)
    {
        var configSetName = configSetNameProvider.GetConfigSetName();
        var collectorMode = collectorModeProvider.GetCollectorMode();

        var dataCollectionsConfig = await dataCollectionsConfigProvider.GetDataCollectionsConfigAsync(configSetName, cancellationToken);
        var dataGroups = dataCollectionsConfig.GroupBy(x => x.DataGroupName).Select(x => new { DataGroupName = x.Key, DataCollectionsConfig = x });

        var tasks = dataGroups.Select(x => CollectGroupAsync(collectorMode, x.DataCollectionsConfig, cancellationToken)).ToList();

        await Task.WhenAll(tasks);

        return tasks.SelectMany(x => x.Result).ToList();
    }

    private async Task<IEnumerable<string>> CollectGroupAsync(CollectorMode collectorMode, IEnumerable<DataCollectionConfig> dataCollectionsConfig, CancellationToken cancellationToken)
    {
        var result = new List<string>();
        foreach (var dataCollectionConfig in dataCollectionsConfig)
        {
            try
            {
                var (newFileNames, collectionFileInfos) = await dataCollector.CollectDataAsync(collectorMode, dataCollectionConfig, cancellationToken);

                if (collectorMode == CollectorMode.Collect)
                {
                    await dataCollector.GarbageCollectDataAsync(dataCollectionConfig, newFileNames, collectionFileInfos, cancellationToken);
                }
            }
            catch
            {
                result.Add(dataCollectionConfig.DataCollectionName);
                // Give other DataCollections a chance
            }
        }

        return result;
    }
}