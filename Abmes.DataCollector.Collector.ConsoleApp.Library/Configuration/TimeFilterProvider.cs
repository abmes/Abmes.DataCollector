using Abmes.DataCollector.Collector.Common.Configuration;

namespace Abmes.DataCollector.Collector.ConsoleApp.Configuration;

public class TimeFilterProvider : ITimeFilterProvider
{
    private readonly IBootstrapper _bootstrapper;

    public TimeFilterProvider(IBootstrapper bootstrapper)
    {
        _bootstrapper = bootstrapper;
    }
    public string GetTimeFilter()
    {
        var args = Environment.GetCommandLineArgs();

        return _bootstrapper.TimeFilter ?? ((args.Length <= 4) ? null : args[4]);
    }
}
