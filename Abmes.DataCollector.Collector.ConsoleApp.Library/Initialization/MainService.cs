using Abmes.DataCollector.Collector.Common.Collecting;
using Abmes.DataCollector.Collector.Common.Configuration;
using System.Threading;
using System.Threading.Tasks;

namespace Abmes.DataCollector.Collector.ConsoleApp.Initialization
{
    public class MainService : IMainService
    {
        private readonly IMainCollector _mainCollector;
        private readonly IConfigSetNameProvider _configSetNameProvider;

        public MainService(
            IMainCollector mainCollector,
            IConfigSetNameProvider configSetNameProvider)
        {
            _mainCollector = mainCollector;
            _configSetNameProvider = configSetNameProvider;
        }

        public async Task MainAsync(CancellationToken cancellationToken)
        {
            var configSetName = _configSetNameProvider.GetConfigSetName();
            await _mainCollector.CollectAsync(configSetName, cancellationToken);
        }
    }
}
