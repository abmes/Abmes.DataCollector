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
        private readonly IDataFilterProvider _dataFilterProvider;
        private readonly IDataCollectionsConfigProvider _DataCollectionsConfigProvider;

        public FilteredDataCollectionsConfigProvider(
            IDataFilterProvider DataFilterProvider,
            IDataCollectionsConfigProvider DataCollectionsConfigProvider)
        {
            _dataFilterProvider = DataFilterProvider;
            _DataCollectionsConfigProvider = DataCollectionsConfigProvider;
        }

        public async Task<IEnumerable<DataCollectionConfig>> GetDataCollectionsConfigAsync(string configSetName, CancellationToken cancellationToken)
        {
            var filter = await _dataFilterProvider.GetDataFilterAsync(cancellationToken);
            var result = await _DataCollectionsConfigProvider.GetDataCollectionsConfigAsync(configSetName, cancellationToken);
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
