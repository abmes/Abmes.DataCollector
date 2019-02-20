using Abmes.DataCollector.Common.Configuration;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace Abmes.DataCollector.Collector.Common.Configuration
{
    public class DataCollectionsConfigProvider : IDataCollectionsConfigProvider
    {
        private const string DataCollectionsConfigFileName = "DataCollectionsConfig.json";

        private readonly IDataCollectionsJsonConfigsProvider _dataCollectJsonConfigsProvider;
        private readonly IConfigProvider _configProvider;

        public DataCollectionsConfigProvider(
            IDataCollectionsJsonConfigsProvider dataCollectJsonConfigsProvider,
            IConfigProvider configProvider)
        {
            _dataCollectJsonConfigsProvider = dataCollectJsonConfigsProvider;
            _configProvider = configProvider;
        }

        public async Task<IEnumerable<DataCollectionConfig>> GetDataCollectionsConfigAsync(string configSetName, CancellationToken cancellationToken)
        {
            var configName = (configSetName + "/" + DataCollectionsConfigFileName).TrimStart('/');
            var json = await _configProvider.GetConfigContentAsync(configName, cancellationToken);
            return _dataCollectJsonConfigsProvider.GetDataCollectionsConfig(json);
        }
    }
}
