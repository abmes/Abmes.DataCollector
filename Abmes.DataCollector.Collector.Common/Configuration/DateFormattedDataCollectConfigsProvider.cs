using Abmes.DataCollector.Collector.Common.Misc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Abmes.DataCollector.Collector.Common.Configuration
{
    public class DateFormattedDataCollectConfigsProvider : IDataCollectConfigsProvider
    {
        private readonly IDateFormattedDataCollectConfigProvider _dateFormattedDataCollectConfigProvider;
        private readonly IDataCollectConfigsProvider _dataCollectConfigsProvider;

        public DateFormattedDataCollectConfigsProvider(
            IDateFormattedDataCollectConfigProvider dateFormattedDataCollectConfigProvider,
            IDataCollectConfigsProvider DataCollectConfigsProvider)
        {
            _dateFormattedDataCollectConfigProvider = dateFormattedDataCollectConfigProvider;
            _dataCollectConfigsProvider = DataCollectConfigsProvider;
        }

        public async Task<IEnumerable<DataCollectConfig>> GetDataCollectConfigsAsync(string configSetName, CancellationToken cancellationToken)
        {
            var DatasCollectConfig = await _dataCollectConfigsProvider.GetDataCollectConfigsAsync(configSetName, cancellationToken);
            return DatasCollectConfig.Select(x => _dateFormattedDataCollectConfigProvider.GetConfig(x));
        }
    }
}
