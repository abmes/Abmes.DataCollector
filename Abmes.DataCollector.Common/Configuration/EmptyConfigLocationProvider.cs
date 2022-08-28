namespace Abmes.DataCollector.Common.Configuration;

public class EmptyConfigLocationProvider : IConfigLocationProvider
{
    public string? GetConfigLocation()
    {
        return null;
    }
}
