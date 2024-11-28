namespace Abmes.DataCollector.Common.Services.Ports.Configuration;

public interface IConfigProvider
{
    Task<string> GetConfigContentAsync(string configName, CancellationToken cancellationToken);
}
