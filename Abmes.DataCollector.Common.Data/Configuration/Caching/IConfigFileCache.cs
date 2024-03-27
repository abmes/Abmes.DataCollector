namespace Abmes.DataCollector.Common.Data.Configuration.Caching;

public interface IConfigFileCache
{
    Task<string> GetConfigFileContentAsync(string fileName, IConfigProvider configFileProvider, CancellationToken cancellationToken);
}
