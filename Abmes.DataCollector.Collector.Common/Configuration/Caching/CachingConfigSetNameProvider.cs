namespace Abmes.DataCollector.Collector.Common.Configuration.Caching;

public class CachingConfigSetNameProvider(
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
