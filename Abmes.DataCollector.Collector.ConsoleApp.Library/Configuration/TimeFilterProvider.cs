using Abmes.DataCollector.Collector.Common.Configuration;

namespace Abmes.DataCollector.Collector.ConsoleApp.Configuration;

public class TimeFilterProvider(
    IBootstrapper bootstrapper) : ITimeFilterProvider
{
    public string GetTimeFilter()
    {
        var args = Environment.GetCommandLineArgs();

        return string.IsNullOrEmpty(bootstrapper.TimeFilter) ? ((args.Length <= 4) ? string.Empty : args[4]) : bootstrapper.TimeFilter;
    }
}
