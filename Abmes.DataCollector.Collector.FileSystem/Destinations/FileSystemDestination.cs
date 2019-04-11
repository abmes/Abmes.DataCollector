using Abmes.DataCollector.Collector.Common.Configuration;
using Abmes.DataCollector.Collector.Common.Destinations;
using Abmes.DataCollector.Common.FileSystem.Storage;
using Abmes.DataCollector.Utils;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Security.Cryptography;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Abmes.DataCollector.Collector.FileSystem.Destinations
{
    public class FileSystemDestination : IFileSystemDestination
    {
        private readonly IFileSystemCommonStorage _fileSystemCommonStorage;

        public DestinationConfig DestinationConfig { get; }

        public FileSystemDestination(
            DestinationConfig destinationConfig,
            IFileSystemCommonStorage FileSystemCommonStorage)
        {
            DestinationConfig = destinationConfig;
            _fileSystemCommonStorage = FileSystemCommonStorage;
        }

        public bool CanGarbageCollect()
        {
            return true;
        }

        public async Task CollectAsync(string collectUrl, IEnumerable<KeyValuePair<string, string>> collectHeaders, IIdentityServiceClientInfo collectIdentityServiceClientInfo, string dataCollectionName, string fileName, TimeSpan timeout, bool finishWait, int tryNo, CancellationToken cancellationToken)
        {
            using (var response = await HttpUtils.SendAsync(collectUrl, HttpMethod.Get, collectUrl, collectHeaders, null, timeout, null, HttpCompletionOption.ResponseHeadersRead, cancellationToken))
            {
                var sourceMD5 = response.ContentMD5();

                var fullFileName = GetFullFileName(dataCollectionName, fileName);

                Directory.CreateDirectory(Path.GetDirectoryName(fullFileName));

                using (var sourceStream = await response.Content.ReadAsStreamAsync())
                {
                    using (var fileStream = new FileStream(fullFileName, FileMode.CreateNew))
                    {
                        await sourceStream.CopyToAsync(fileStream);
                    }
                }

                using (var fileStream = new FileStream(fullFileName, FileMode.Open, FileAccess.Read))
                {
                    var newMD5 = await CopyUtils.GetMD5HashAsync(fileStream, 4096, cancellationToken);

                    if ((!string.IsNullOrEmpty(sourceMD5)) &&
                        (!string.Equals(newMD5, sourceMD5, StringComparison.InvariantCultureIgnoreCase)))
                    {
                        throw new Exception("Invalid destination MD5");
                    }

                    File.WriteAllText(fullFileName + ".md5", newMD5);
                }
            }
        }

        public async Task<IEnumerable<string>> GetDataCollectionFileNamesAsync(string dataCollectionName, CancellationToken cancellationToken)
        {
            return await _fileSystemCommonStorage.GetDataCollectionFileNamesAsync(null, null, DestinationConfig.RootBase(), DestinationConfig.RootDir('\\', false), dataCollectionName, null, cancellationToken);
        }

        public async Task GarbageCollectDataCollectionFileAsync(string dataCollectionName, string fileName, CancellationToken cancellationToken)
        {
            var fullFileName = GetFullFileName(dataCollectionName, fileName);
            File.Delete(fullFileName);
            File.Delete(GetMD5FileName(fullFileName));
            await Task.CompletedTask;
        }

        private string GetMD5FileName(string fileName)
        {
            return fileName + ".md5";
        }

        private string GetFullFileName(string dataCollectionName, string fileName)
        {
            return Path.Combine(DestinationConfig.Root, dataCollectionName, fileName.Replace("/", "\\"));
        }

        private async Task<string> GetFileMD5Async(string fullFileName, CancellationToken cancellationToken)
        {
            var md5FileName = GetMD5FileName(fullFileName);

            return File.Exists(md5FileName) ? await File.ReadAllTextAsync(md5FileName, cancellationToken) : null;
        }

        public async Task<bool> AcceptsFileAsync(string dataCollectionName, string name, long? size, string md5, CancellationToken cancellationToken)
        {
            var fullFileName = GetFullFileName(dataCollectionName, name);

            if (DestinationConfig.OverrideFiles)
                return true;

            if (!(File.Exists(fullFileName)))
                return true;

            if ((size.HasValue) && (size.Value != new FileInfo(fullFileName).Length))
                return true;

            if (!string.IsNullOrEmpty(md5))
            {
                var destMD5 = await GetFileMD5Async(fullFileName, cancellationToken);

                if (md5 != destMD5)
                    return true;
            }

            return false;
        }
    }
}
