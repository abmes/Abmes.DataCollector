using Abmes.DataCollector.Collector.ConsoleApp.Initialization;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace Abmes.DataCollector.Collector.ConsoleApp
{
    class Program
    {
        static int DelayedExitCode(int exitCode, int delaySeconds = 0)
        {
            if (delaySeconds > 0)
            {
                Console.WriteLine($"Exitting after {delaySeconds} seconds ...");
                Task.Delay(TimeSpan.FromSeconds(delaySeconds)).Wait();
            }

            return exitCode;
        }

        static async Task<int> Main(string[] args)
        {
            try
            {
                await 
                    Initializer
                    .GetMainService()
                    .MainAsync(CancellationToken.None);

                return DelayedExitCode(0, 5);
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
                return DelayedExitCode(1, 5);
            }
        }
    }
}
