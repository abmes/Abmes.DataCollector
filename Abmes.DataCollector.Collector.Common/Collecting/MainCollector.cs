using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Threading;
using Abmes.DataCollector.Collector.Common.Configuration;

namespace Abmes.DataCollector.Collector.Common.Collecting
{
    public class MainCollector : IMainCollector
    {
        private readonly IConfigSetNameProvider _configSetNameProvider;
        private readonly IDataCollectionsConfigProvider _dataCollectionsConfigProvider;
        private readonly IDataCollector _dataCollector;
        private readonly ICollectorModeProvider _collectorModeProvider;

        public MainCollector(
            IConfigSetNameProvider configSetNameProvider,
            IDataCollectionsConfigProvider dataCollectionsConfigProvider,
            IDataCollector dataCollector,
            ICollectorModeProvider collectorModeProvider)
        {
            _configSetNameProvider = configSetNameProvider;
            _dataCollectionsConfigProvider = dataCollectionsConfigProvider;
            _dataCollector = dataCollector;
            _collectorModeProvider = collectorModeProvider;
        }

        public async Task<bool> CollectAsync(CancellationToken cancellationToken)
        {
            var configSetName = _configSetNameProvider.GetConfigSetName();
            var collectorMode = _collectorModeProvider.GetCollectorMode();

            var dataCollectionsConfig = await _dataCollectionsConfigProvider.GetDataCollectionsConfigAsync(configSetName, cancellationToken);
            var dataGroups = dataCollectionsConfig.GroupBy(x => x.DataGroupName).Select(x => new { DataGroupName = x.Key, DataCollectionsConfig = x });

            var tasks = dataGroups.Select(x => CollectGroupAsync(x.DataGroupName, collectorMode, x.DataCollectionsConfig, cancellationToken)).ToList();

            await Task.WhenAll(tasks);

            return (tasks.Any() ? tasks.Select(x => x.Result).Min() : false);
        }

        private async Task<bool> CollectGroupAsync(string groupName, CollectorMode collectorMode, IEnumerable<DataCollectionConfig> dataCollectionsConfig, CancellationToken cancellationToken)
        {
            var result = true;
            foreach (var dataCollectionConfig in dataCollectionsConfig)
            {
                try
                {
                    await _dataCollector.CollectDataAsync(collectorMode, dataCollectionConfig, cancellationToken);

                    if (collectorMode == CollectorMode.Collect)
                    {
                        await _dataCollector.GarbageCollectDataAsync(dataCollectionConfig, cancellationToken);
                    }
                }
                catch
                {
                    result = false;
                    // Give other DataCollections a chance
                }
            }

            return result;
        }
    }
}