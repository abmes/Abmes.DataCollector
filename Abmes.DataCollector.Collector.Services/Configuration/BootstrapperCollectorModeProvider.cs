namespace Abmes.DataCollector.Collector.Services.Configuration;

public class BootstrapperCollectorModeProvider(
    IBootstrapper bootstrapper) : ICollectorModeProvider
{
    public CollectorMode GetCollectorMode()
    {
        return bootstrapper.CollectorMode;
    }
}
