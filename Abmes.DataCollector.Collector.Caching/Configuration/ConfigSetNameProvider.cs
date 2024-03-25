using Abmes.DataCollector.Collector.Services.Configuration;

namespace Abmes.DataCollector.Collector.Caching.Configuration;

public class ConfigSetNameProvider(
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
