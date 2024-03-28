using Abmes.DataCollector.Collector.Services.Abstractions.Configuration;

namespace Abmes.DataCollector.Collector.Services.Configuration;

public class BootstrapperCollectorModeProvider(  // todo: this should be in web.library
    IBootstrapper bootstrapper) : ICollectorModeProvider
{
    public CollectorMode GetCollectorMode()
    {
        return bootstrapper.CollectorMode;
    }
}
