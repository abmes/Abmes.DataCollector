namespace Abmes.DataCollector.Collector.Services.Configuration;

public class FilteredDataCollectionsConfigProvider(
    IDataCollectionsFilterProvider dataFilterProvider,
    IDataCollectionsConfigProvider dataCollectionsConfigProvider) : IDataCollectionsConfigProvider
{
    public async Task<IEnumerable<DataCollectionConfig>> GetDataCollectionsConfigAsync(string configSetName, CancellationToken cancellationToken)
    {
        var filter = await dataFilterProvider.GetDataCollectionsFilterAsync(cancellationToken);
        var result = await dataCollectionsConfigProvider.GetDataCollectionsConfigAsync(configSetName, cancellationToken);
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
