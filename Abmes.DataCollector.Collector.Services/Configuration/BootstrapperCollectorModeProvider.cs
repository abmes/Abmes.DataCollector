using Abmes.DataCollector.Collector.Services.Ports.AppConfig;
using Abmes.DataCollector.Collector.Services.Ports.Configuration;

namespace Abmes.DataCollector.Collector.Services.Configuration;

public class BootstrapperCollectorModeProvider(
    IBootstrapper bootstrapper)
    : ICollectorModeProvider
{
    public CollectorMode GetCollectorMode()
    {
        return bootstrapper.CollectorMode;
    }
}
