using Abmes.DataCollector.Collector.Common.Collecting;
using Abmes.DataCollector.Collector.Common.Configuration;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace Abmes.DataCollector.Collector.ConsoleApp.Initialization
{
    public class MainService : IMainService
    {
        private readonly IMainCollector _mainCollector;
        private readonly IBootstrapper _bootstrapper;

        public MainService(
            IMainCollector mainCollector,
            IBootstrapper bootstrapper)
        {
            _mainCollector = mainCollector;
            _bootstrapper = bootstrapper;
        }

        public async Task MainAsync(CancellationToken cancellationToken, Action<IBootstrapper> bootstrap = null)
        {
            bootstrap?.Invoke(_bootstrapper);

            await _mainCollector.CollectAsync(cancellationToken);
        }
    }
}
