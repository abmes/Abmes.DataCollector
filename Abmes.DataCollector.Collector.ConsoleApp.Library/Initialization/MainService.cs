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

        public async Task<int> MainAsync(CancellationToken cancellationToken, Action<IBootstrapper> bootstrap = null)
        {
            try
            {
                bootstrap?.Invoke(_bootstrapper);

                await _mainCollector.CollectAsync(cancellationToken);

                return DelayedExitCode(0, 5);
            }
            catch (Exception e)
            {
                System.Console.WriteLine(e.Message);
                return DelayedExitCode(1, 5);
            }
        }

        private int DelayedExitCode(int exitCode, int delaySeconds = 0)
        {
            if (delaySeconds > 0)
            {
                System.Console.WriteLine($"Exitting after {delaySeconds} seconds ...");
                Task.Delay(TimeSpan.FromSeconds(delaySeconds)).Wait();
            }

#if DEBUG
            Task.Delay(500).Wait();
            System.Console.WriteLine("Press any key to quit...");
            System.Console.ReadKey();
#endif

            return exitCode;
        }
    }
}
