using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Abmes.DataCollector.Collector.Common.Configuration
{
    public class FilteredDataCollectionsConfigProvider : IDataCollectionsConfigProvider
    {
        private readonly IDataCollectionsFilterProvider _dataFilterProvider;
        private readonly IDataCollectionsConfigProvider _dataCollectionsConfigProvider;

        public FilteredDataCollectionsConfigProvider(
            IDataCollectionsFilterProvider dataFilterProvider,
            IDataCollectionsConfigProvider dataCollectionsConfigProvider)
        {
            _dataFilterProvider = dataFilterProvider;
            _dataCollectionsConfigProvider = dataCollectionsConfigProvider;
        }

        public async Task<IEnumerable<DataCollectionConfig>> GetDataCollectionsConfigAsync(string configSetName, CancellationToken cancellationToken)
        {
            var filter = await _dataFilterProvider.GetDataCollectionsFilterAsync(cancellationToken);
            var result = await _dataCollectionsConfigProvider.GetDataCollectionsConfigAsync(configSetName, cancellationToken);
            return result.Where(x => DataMatchesFilter(x, filter));
        }

        private bool DataMatchesFilter(DataCollectionConfig x, string filter)
        {
            return 
                string.IsNullOrEmpty(filter) || 
                filter.Split(';', ',').Contains(x.DataCollectionName, StringComparer.OrdinalIgnoreCase);
        }
    }
}
