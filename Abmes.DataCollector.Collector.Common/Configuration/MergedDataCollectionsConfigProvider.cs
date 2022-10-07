namespace Abmes.DataCollector.Collector.Common.Configuration;

public class MergedDataCollectionsConfigProvider : IDataCollectionsConfigProvider
{
    private readonly IDataCollectionsConfigProvider _dataCollectionsConfigProvider;
    private readonly IMergedDataCollectionConfigProvider _mergedDataCollectionConfigProvider;

    public MergedDataCollectionsConfigProvider(
        IDataCollectionsConfigProvider dataCollectionsConfigProvider,
        IMergedDataCollectionConfigProvider mergedDataCollectionConfigProvider)
    {
        _dataCollectionsConfigProvider = dataCollectionsConfigProvider;
        _mergedDataCollectionConfigProvider = mergedDataCollectionConfigProvider;
    }

    public async Task<IEnumerable<DataCollectionConfig>> GetDataCollectionsConfigAsync(string configSetName, CancellationToken cancellationToken)
    {
        var dataCollectionsConfig = await _dataCollectionsConfigProvider.GetDataCollectionsConfigAsync(configSetName, cancellationToken);

        var template = dataCollectionsConfig.Where(x => x.DataCollectionName == "*").FirstOrDefault();

        return
            template is null
            ?
            dataCollectionsConfig
            :
            dataCollectionsConfig
                .Where(x => x.DataCollectionName != "*")
                .Select(x => _mergedDataCollectionConfigProvider.GetConfig(x, template));
    }
}
