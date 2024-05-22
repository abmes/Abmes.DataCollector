namespace Abmes.DataCollector.Collector.Services.Configuration;

public class MergedDataCollectionsConfigProvider(
    IDataCollectionsConfigProvider dataCollectionsConfigProvider,
    IMergedDataCollectionConfigProvider mergedDataCollectionConfigProvider) : IDataCollectionsConfigProvider
{
    public async Task<IEnumerable<DataCollectionConfig>> GetDataCollectionsConfigAsync(string configSetName, CancellationToken cancellationToken)
    {
        var dataCollectionsConfig = await dataCollectionsConfigProvider.GetDataCollectionsConfigAsync(configSetName, cancellationToken);

        var template = dataCollectionsConfig.Where(x => x.DataCollectionName == "*").FirstOrDefault();

        return
            template is null
            ? dataCollectionsConfig
            : dataCollectionsConfig
                .Where(x => x.DataCollectionName != "*")
                .Select(x => mergedDataCollectionConfigProvider.GetConfig(x, template));
    }
}
