using Abmes.DataCollector.Common.Configuration;
using System.Text;

namespace Abmes.DataCollector.Common.FileSystem.Configuration
{
    public class ConfigLoader : IConfigLoader
    {
        private readonly IFileSystemAppSettings _fileSystemAppSettings;

        public ConfigLoader(
            IFileSystemAppSettings fileSystemAppSettings)
        {
            _fileSystemAppSettings = fileSystemAppSettings;
        }

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
            return await GetConfigContentAsync(configName, _fileSystemAppSettings.FileSystemConfigStorageRoot, cancellationToken);
        }

        public async Task<string> GetConfigContentAsync(string configName, string location, CancellationToken cancellationToken)
        {
            var fileName = System.IO.Path.Combine(location, configName.Replace("/", "\\"));

            return await GetFileContentAsync(fileName, cancellationToken);
        }

        private static async Task<string> GetFileContentAsync(string fileName, CancellationToken cancellationToken)
        {
            using (var fileStream = new System.IO.FileStream(fileName, System.IO.FileMode.Open))
            {
                using (var reader = new System.IO.StreamReader(fileStream, Encoding.UTF8))
                {
                    return await reader.ReadToEndAsync();
                }
            }
        }
    }
}
