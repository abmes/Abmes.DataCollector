using Abmes.DataCollector.Collector.ConsoleApp.Initialization;
using System;
using System.Threading;
using System.Threading.Tasks;
using static System.Console;

namespace Abmes.DataCollector.Collector.ConsoleApp
{
    class Program
    {
        static async Task<int> Main(string[] args)
        {
            return await Initializer.GetMainService().MainAsync(CancellationToken.None);
        }
    }
}
