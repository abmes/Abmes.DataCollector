using Abmes.DataCollector.Collector.ConsoleApp.Initialization;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace Abmes.DataCollector.Collector.ConsoleApp
{
    class Program
    {
        static async Task<int> Main(string[] args)
        {
            try
            {
                await 
                    Initializer
                    .GetMainService()
                    .MainAsync(CancellationToken.None);

                return 0;
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
                return 1;
            }
        }
    }
}
