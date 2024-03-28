using Abmes.DataCollector.Collector.Services.Abstractions.AppConfig;

namespace Abmes.DataCollector.Collector.App.Library.AppConfig;

public class TimeFilterProvider(
    IBootstrapper bootstrapper) : ITimeFilterProvider
{
    public string GetTimeFilter()
    {
        var args = Environment.GetCommandLineArgs();

        return
            string.IsNullOrEmpty(bootstrapper.TimeFilter)
            ? ((args.Length <= 4) ? string.Empty : args[4])
            : bootstrapper.TimeFilter;
    }
}
