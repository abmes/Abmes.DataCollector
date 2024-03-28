namespace Abmes.DataCollector.Common.Data.Configuration;

public class EmptyConfigLocationProvider : IConfigLocationProvider
{
    public string? GetConfigLocation()
    {
        return null;
    }
}
