namespace Abmes.DataCollector.Collector.Common.Configuration;

public class BootstrapperCollectorModeProvider(
    IBootstrapper bootstrapper) : ICollectorModeProvider
{
    public CollectorMode GetCollectorMode()
    {
        return bootstrapper.CollectorMode;
    }
}
