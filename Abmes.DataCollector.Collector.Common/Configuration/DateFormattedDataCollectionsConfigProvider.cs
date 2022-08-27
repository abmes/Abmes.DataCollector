namespace Abmes.DataCollector.Collector.Common.Configuration
{
    public class DateFormattedDataCollectionsConfigProvider : IDataCollectionsConfigProvider
    {
        private readonly IDateFormattedDataCollectionConfigProvider _dateFormattedDataCollectionConfigProvider;
        private readonly IDataCollectionsConfigProvider _dataCollectionsConfigProvider;

        public DateFormattedDataCollectionsConfigProvider(
            IDateFormattedDataCollectionConfigProvider dateFormattedDataCollectionConfigProvider,
            IDataCollectionsConfigProvider dataCollectionsConfigProvider)
        {
            _dateFormattedDataCollectionConfigProvider = dateFormattedDataCollectionConfigProvider;
            _dataCollectionsConfigProvider = dataCollectionsConfigProvider;
        }

        public async Task<IEnumerable<DataCollectionConfig>> GetDataCollectionsConfigAsync(string configSetName, CancellationToken cancellationToken)
        {
            var dataCollectionsConfig = await _dataCollectionsConfigProvider.GetDataCollectionsConfigAsync(configSetName, cancellationToken);
            return dataCollectionsConfig.Select(x => _dateFormattedDataCollectionConfigProvider.GetConfig(x));
        }
    }
}
