using Abmes.DataCollector.Collector.ConsoleApp.Initialization;

namespace Abmes.DataCollector.Collector.ConsoleApp;

class Program
{
    static async Task<int> Main(string[] args)
    {
        return await Initializer.GetMainService().MainAsync(CancellationToken.None);
    }
}
