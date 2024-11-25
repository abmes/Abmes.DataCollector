using Abmes.DataCollector.Collector.Services.Configuration;
using Abmes.DataCollector.Collector.Services.Contracts;
using Abmes.DataCollector.Collector.Services.Contracts.AppConfig;

namespace Abmes.DataCollector.Collector.App.Library.Configuration;

public class CollectorModeProvider(
    IBootstrapper bootstrapper) : ICollectorModeProvider
{
    public CollectorMode GetCollectorMode()
    {
        var args = Environment.GetCommandLineArgs();

        return
            bootstrapper.CollectorMode != CollectorMode.None
            ? bootstrapper.CollectorMode
            : args.Length > 3
                ? Enum.Parse<CollectorMode>(args[3], true)
                : CollectorMode.Collect;
    }
}
