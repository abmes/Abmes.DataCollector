namespace Abmes.DataCollector.Collector.Common.Configuration;

public class BootstrapperTimeFilterProvider(
    IBootstrapper bootstrapper) : ITimeFilterProvider
{
    public string GetTimeFilter()
    {
        return bootstrapper.TimeFilter;
    }
}
