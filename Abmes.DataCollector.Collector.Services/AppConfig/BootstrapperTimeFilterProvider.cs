using Abmes.DataCollector.Collector.Services.Contracts.AppConfig;

namespace Abmes.DataCollector.Collector.Services.AppConfig;

public class BootstrapperTimeFilterProvider(
    IBootstrapper bootstrapper) : ITimeFilterProvider
{
    public string GetTimeFilter()
    {
        return bootstrapper.TimeFilter;
    }
}
