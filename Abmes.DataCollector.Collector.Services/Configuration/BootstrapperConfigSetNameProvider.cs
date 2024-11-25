using Abmes.DataCollector.Collector.Common.Configuration;
using Abmes.DataCollector.Collector.Services.Contracts.AppConfig;

namespace Abmes.DataCollector.Collector.Services.Configuration;

public class BootstrapperConfigSetNameProvider(
    IBootstrapper bootstrapper) : IConfigSetNameProvider
{
    public string GetConfigSetName()
    {
        return bootstrapper.ConfigSetName;
    }
}
