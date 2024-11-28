using Abmes.DataCollector.Common.Services.Ports.Configuration;

namespace Abmes.DataCollector.Common.Services.Configuration.Caching;

public interface IConfigFileCache
{
    Task<string> GetConfigFileContentAsync(string fileName, IConfigProvider configFileProvider, CancellationToken cancellationToken);
}
