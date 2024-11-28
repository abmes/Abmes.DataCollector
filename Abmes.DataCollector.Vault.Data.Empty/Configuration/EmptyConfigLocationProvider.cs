using Abmes.DataCollector.Common.Services.Ports.Configuration;

namespace Abmes.DataCollector.Vault.Data.Empty.Configuration;

public class EmptyConfigLocationProvider : IConfigLocationProvider
{
    public string? GetConfigLocation()
    {
        return null;
    }
}
