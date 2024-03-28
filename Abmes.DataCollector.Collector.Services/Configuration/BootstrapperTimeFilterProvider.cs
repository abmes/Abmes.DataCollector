using Abmes.DataCollector.Collector.Services.Abstractions.Configuration;

namespace Abmes.DataCollector.Collector.Services.Configuration;

public class BootstrapperTimeFilterProvider(  // todo: this should be in web.library
    IBootstrapper bootstrapper) : ITimeFilterProvider
{
    public string GetTimeFilter()
    {
        return bootstrapper.TimeFilter;
    }
}
