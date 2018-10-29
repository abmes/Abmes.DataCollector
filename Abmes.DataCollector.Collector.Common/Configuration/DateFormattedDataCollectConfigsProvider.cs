using Abmes.DataCollector.Collector.Common.Misc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

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
            var DatasCollectConfig = await _dataCollectionsConfigProvider.GetDataCollectionsConfigAsync(configSetName, cancellationToken);
            return DatasCollectConfig.Select(x => _dateFormattedDataCollectionConfigProvider.GetConfig(x));
        }
    }
}
