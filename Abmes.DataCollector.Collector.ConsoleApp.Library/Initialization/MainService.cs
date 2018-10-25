using Abmes.DataCollector.Collector.Common.Collecting;
using System.Threading;
using System.Threading.Tasks;

namespace Abmes.DataCollector.Collector.ConsoleApp.Initialization
{
    public class MainService : IMainService
    {
        private readonly IMainCollector _mainCollector;

        public MainService(IMainCollector mainCollector)
        {
            _mainCollector = mainCollector;
        }

        public async Task MainAsync(CancellationToken cancellationToken)
        {
            await _mainCollector.CollectAsync(cancellationToken);
        }
    }
}
