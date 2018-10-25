using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace Abmes.DataCollector.Collector.Common.Configuration
{
    public class DataCollectConfigsProvider : IDataCollectConfigsProvider
    {
        private const string DataCollectConfigsFileName = "DataCollectConfigs.json";

        private readonly IDataCollectJsonConfigsProvider _dataCollectJsonConfigsProvider;
        private readonly IConfigProvider _configProvider;

        public DataCollectConfigsProvider(
            IDataCollectJsonConfigsProvider dataCollectJsonConfigsProvider,
            IConfigProvider configProvider)
        {
            _dataCollectJsonConfigsProvider = dataCollectJsonConfigsProvider;
            _configProvider = configProvider;
        }

        public async Task<IEnumerable<DataCollectConfig>> GetDataCollectConfigsAsync(string configSetName, CancellationToken cancellationToken)
        {
            var configName = (configSetName + "/" + DataCollectConfigsFileName).TrimStart('/');
            var json = await _configProvider.GetConfigContentAsync(configName, cancellationToken);
            return _dataCollectJsonConfigsProvider.GetDataCollectConfigs(json);
        }
    }
}
