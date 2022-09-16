using Abmes.DataCollector.Collector.Common.Configuration;

namespace Abmes.DataCollector.Collector.Common.Collecting;

public class MainCollector : IMainCollector
{
    private readonly IConfigSetNameProvider _configSetNameProvider;
    private readonly IDataCollectionsConfigProvider _dataCollectionsConfigProvider;
    private readonly IDataCollector _dataCollector;
    private readonly ICollectorModeProvider _collectorModeProvider;

    public MainCollector(
        IConfigSetNameProvider configSetNameProvider,
        IDataCollectionsConfigProvider dataCollectionsConfigProvider,
        IDataCollector dataCollector,
        ICollectorModeProvider collectorModeProvider)
    {
        _configSetNameProvider = configSetNameProvider;
        _dataCollectionsConfigProvider = dataCollectionsConfigProvider;
        _dataCollector = dataCollector;
        _collectorModeProvider = collectorModeProvider;
    }

    public async Task<IEnumerable<string>> CollectAsync(CancellationToken cancellationToken)
    {
        var configSetName = _configSetNameProvider.GetConfigSetName();
        var collectorMode = _collectorModeProvider.GetCollectorMode();

        var dataCollectionsConfig = await _dataCollectionsConfigProvider.GetDataCollectionsConfigAsync(configSetName, cancellationToken);
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
                var (newFileNames, collectionFileInfos) = await _dataCollector.CollectDataAsync(collectorMode, dataCollectionConfig, cancellationToken);

                if (collectorMode == CollectorMode.Collect)
                {
                    await _dataCollector.GarbageCollectDataAsync(dataCollectionConfig, newFileNames, collectionFileInfos, cancellationToken);
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