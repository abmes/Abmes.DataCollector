namespace Abmes.DataCollector.Common.Data.Configuration;

public class EmptyConfigLocationProvider : IConfigLocationProvider  // todo: this should be in vault.web. library and maybe in collector.web.library
{
    public string? GetConfigLocation()
    {
        return null;
    }
}
