using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Threading;
using Abmes.DataCollector.Collector.Common.Configuration;

namespace Abmes.DataCollector.Collector.Common.Collecting
{
    public class MainCollector : IMainCollector
    {
        private readonly IDataCollectionsConfigProvider _dataCollectionsConfigProvider;
        private readonly IConfigSetNameProvider _configSetNameProvider;
        private readonly IDataCollector _dataCollector;

        public MainCollector(
            IDataCollectionsConfigProvider dataCollectionsConfigProvider,
            IConfigSetNameProvider configSetNameProvider,
            IDataCollector dataCollector)
        {
            _dataCollectionsConfigProvider = dataCollectionsConfigProvider;
            _configSetNameProvider = configSetNameProvider;
            _dataCollector = dataCollector;
        }

        public async Task CollectAsync(CancellationToken cancellationToken)
        {
            var configSetName = _configSetNameProvider.GetConfigSetName();
            var DatasCollectConfig = await _dataCollectionsConfigProvider.GetDataCollectionsConfigAsync(configSetName, cancellationToken);
            var DataGroups = DatasCollectConfig.GroupBy(x => x.DataGroupName).Select(x => new { DataGroupName = x.Key, DatasCollectConfig = x });

            await Task.WhenAll(DataGroups.Select(x => CollectGroupAsync(x.DataGroupName, x.DatasCollectConfig, cancellationToken)));
        }

        private async Task CollectGroupAsync(string groupName, IEnumerable<DataCollectionConfig> datasCollectConfig, CancellationToken cancellationToken)
        {
            foreach (var dataCollectionConfig in datasCollectConfig)
            {
                try
                {
                    await _dataCollector.CollectDataAsync(dataCollectionConfig, cancellationToken);
                    await _dataCollector.GarbageCollectDataAsync(dataCollectionConfig, cancellationToken);
                }
                catch
                {
                    // Give other Datas a chance
                }
            }
        }
    }
}