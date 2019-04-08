using Abmes.DataCollector.Common.Configuration;
using System;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

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

        public bool CanLoadFrom(string storageType)
        {
            return string.Equals(storageType, "FileSystem", StringComparison.InvariantCultureIgnoreCase);
        }

        public async Task<string> GetConfigContentAsync(string configName, CancellationToken cancellationToken)
        {
            var fileName = System.IO.Path.Combine(_fileSystemAppSettings.FileSystemConfigStorageRoot, configName);

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
