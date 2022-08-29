using Abmes.DataCollector.Collector.Common.Configuration;

namespace Abmes.DataCollector.Collector.Caching.Configuration;

public class ConfigSetNameProvider : IConfigSetNameProvider
{
    private readonly IConfigSetNameProvider _configSetNameProvider;
    private string? _configSetName;

    public ConfigSetNameProvider(
        IConfigSetNameProvider configSetNameProvider)
    {
        _configSetNameProvider = configSetNameProvider;
    }

    public string GetConfigSetName()
    {
        if (string.IsNullOrEmpty(_configSetName))
        {
            _configSetName = _configSetNameProvider.GetConfigSetName();
        }

        return _configSetName;
    }
}
