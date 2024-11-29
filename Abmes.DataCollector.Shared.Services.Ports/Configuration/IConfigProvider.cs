namespace Abmes.DataCollector.Shared.Services.Ports.Configuration;

public interface IConfigProvider
{
    Task<string> GetConfigContentAsync(string configName, CancellationToken cancellationToken);
}
