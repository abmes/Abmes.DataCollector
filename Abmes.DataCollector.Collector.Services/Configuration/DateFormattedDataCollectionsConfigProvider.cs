namespace Abmes.DataCollector.Collector.Services.Configuration;

public class DateFormattedDataCollectionsConfigProvider(
    IDateFormattedDataCollectionConfigProvider dateFormattedDataCollectionConfigProvider,
    IDataCollectionsConfigProvider dataCollectionsConfigProvider) : IDataCollectionsConfigProvider
{
    public async Task<IEnumerable<DataCollectionConfig>> GetDataCollectionsConfigAsync(string configSetName, CancellationToken cancellationToken)
    {
        var dataCollectionsConfig = await dataCollectionsConfigProvider.GetDataCollectionsConfigAsync(configSetName, cancellationToken);
        return dataCollectionsConfig.Select(x => dateFormattedDataCollectionConfigProvider.GetConfig(x));
    }
}
