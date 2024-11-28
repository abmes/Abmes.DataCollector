using Abmes.DataCollector.Common.Services.Ports.Configuration;
using System.Text;

namespace Abmes.DataCollector.Common.Data.FileSystem.Configuration;

public class ConfigLoader(
    IFileSystemAppSettings fileSystemAppSettings)
    : IConfigLoader
{
    public bool CanLoadFromStorage(string storageType)
    {
        return string.Equals(storageType, "FileSystem", StringComparison.InvariantCultureIgnoreCase);
    }

    public bool CanLoadFromLocation(string location)
    {
        return System.IO.Path.IsPathFullyQualified(location);
    }

    public async Task<string> GetConfigContentAsync(string configName, CancellationToken cancellationToken)
    {
        return await GetConfigContentAsync(configName, fileSystemAppSettings.FileSystemConfigStorageRoot, cancellationToken);
    }

    public async Task<string> GetConfigContentAsync(string configName, string location, CancellationToken cancellationToken)
    {
        var fileName = System.IO.Path.Combine(location, configName.Replace("/", "\\"));

        return await GetFileContentAsync(fileName, cancellationToken);
    }

    private static async Task<string> GetFileContentAsync(string fileName, CancellationToken cancellationToken)
    {
        await using var fileStream = new FileStream(fileName, FileMode.Open);
        using var reader = new StreamReader(fileStream, Encoding.UTF8);
        return await reader.ReadToEndAsync(cancellationToken);
    }
}
