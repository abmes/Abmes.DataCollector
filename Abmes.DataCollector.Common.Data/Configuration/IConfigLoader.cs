namespace Abmes.DataCollector.Common.Data.Configuration;

public interface IConfigLoader
{
    bool CanLoadFromStorage(string storageType);
    bool CanLoadFromLocation(string location);
    Task<string> GetConfigContentAsync(string configName, CancellationToken cancellationToken);
    Task<string> GetConfigContentAsync(string configName, string location, CancellationToken cancellationToken);
}
