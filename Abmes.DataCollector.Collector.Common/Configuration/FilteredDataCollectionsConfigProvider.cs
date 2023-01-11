namespace Abmes.DataCollector.Collector.Common.Configuration;

public class FilteredDataCollectionsConfigProvider : IDataCollectionsConfigProvider
{
    private readonly IDataCollectionsFilterProvider _dataCollectionsFilterProvider;
    private readonly IDataCollectionsConfigProvider _dataCollectionsConfigProvider;

    public FilteredDataCollectionsConfigProvider(
        IDataCollectionsFilterProvider dataFilterProvider,
        IDataCollectionsConfigProvider dataCollectionsConfigProvider)
    {
        _dataCollectionsFilterProvider = dataFilterProvider;
        _dataCollectionsConfigProvider = dataCollectionsConfigProvider;
    }

    public async Task<IEnumerable<DataCollectionConfig>> GetDataCollectionsConfigAsync(string configSetName, CancellationToken cancellationToken)
    {
        var filter = await _dataCollectionsFilterProvider.GetDataCollectionsFilterAsync(cancellationToken);
        var result = await _dataCollectionsConfigProvider.GetDataCollectionsConfigAsync(configSetName, cancellationToken);
        return result.Where(x => DataMatchesFilter(x, filter));
    }

    private static bool DataMatchesFilter(DataCollectionConfig config, string filter)
    {
        return 
            string.IsNullOrEmpty(filter) ||
            (filter == "*") ||
            filter.Split(';', ',')
            .Where(x => !string.IsNullOrEmpty(x))
            .Select(x => x.Trim())
            .Contains(config.DataCollectionName, StringComparer.OrdinalIgnoreCase);
    }
}
