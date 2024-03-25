namespace Abmes.DataCollector.Collector.Services.Configuration;

public class BootstrapperTimeFilterProvider(
    IBootstrapper bootstrapper) : ITimeFilterProvider
{
    public string GetTimeFilter()
    {
        return bootstrapper.TimeFilter;
    }
}
