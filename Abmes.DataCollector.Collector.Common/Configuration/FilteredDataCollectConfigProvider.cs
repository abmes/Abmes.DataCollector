using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Abmes.DataCollector.Collector.Common.Configuration
{
    public class FilteredDataCollectConfigsProvider : IDataCollectConfigsProvider
    {
        private readonly IDataFilterProvider _dataFilterProvider;
        private readonly IDataCollectConfigsProvider _dataCollectConfigsProvider;

        public FilteredDataCollectConfigsProvider(
            IDataFilterProvider DataFilterProvider,
            IDataCollectConfigsProvider DataCollectConfigsProvider)
        {
            _dataFilterProvider = DataFilterProvider;
            _dataCollectConfigsProvider = DataCollectConfigsProvider;
        }

        public async Task<IEnumerable<DataCollectConfig>> GetDataCollectConfigsAsync(string configSetName, CancellationToken cancellationToken)
        {
            var filter = await _dataFilterProvider.GetDataFilterAsync(cancellationToken);
            var result = await _dataCollectConfigsProvider.GetDataCollectConfigsAsync(configSetName, cancellationToken);
            return result.Where(x => DataMatchesFilter(x, filter));
        }

        private bool DataMatchesFilter(DataCollectConfig x, string filter)
        {
            return 
                string.IsNullOrEmpty(filter) || 
                filter.Split(';', ',').Contains(x.DataCollectionName, StringComparer.OrdinalIgnoreCase);
        }
    }
}
