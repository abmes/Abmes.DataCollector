using Abmes.DataCollector.Collector.Services.Ports.Configuration;

namespace Abmes.DataCollector.Collector.Services.Configuration.ConfigSetName.Caching;

public class ConfigSetNameProviderCachingDecorator(
    IConfigSetNameProvider configSetNameProvider) : IConfigSetNameProvider
{
    private string? _configSetName;

    public string GetConfigSetName()
    {
        if (string.IsNullOrEmpty(_configSetName))
        {
            _configSetName = configSetNameProvider.GetConfigSetName();
        }

        return _configSetName;
    }
}
