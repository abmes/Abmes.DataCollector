using Abmes.DataCollector.Collector.Common.Configuration;
using Abmes.DataCollector.Collector.Services.Abstractions.Configuration;

namespace Abmes.DataCollector.Collector.Services.Configuration;

public class BootstrapperConfigSetNameProvider(  // todo: thisshould be in web.library
    IBootstrapper bootstrapper) : IConfigSetNameProvider
{
    public string GetConfigSetName()
    {
        return bootstrapper.ConfigSetName;
    }
}
