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

        return string.IsNullOrEmpty(_bootstrapper.TimeFilter) ? ((args.Length <= 4) ? string.Empty : args[4]) : _bootstrapper.TimeFilter;
    }
}
