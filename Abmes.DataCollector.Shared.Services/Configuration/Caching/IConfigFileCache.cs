using Abmes.DataCollector.Shared.Services.Ports.Configuration;

namespace Abmes.DataCollector.Shared.Services.Configuration.Caching;

public interface IConfigFileCache
{
    Task<string> GetConfigFileContentAsync(string fileName, IConfigProvider configFileProvider, CancellationToken cancellationToken);
}
