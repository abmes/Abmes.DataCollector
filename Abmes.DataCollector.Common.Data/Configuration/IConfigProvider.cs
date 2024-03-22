namespace Abmes.DataCollector.Common.Data.Configuration;

public interface IConfigProvider
{
    Task<string> GetConfigContentAsync(string configName, CancellationToken cancellationToken);
}
