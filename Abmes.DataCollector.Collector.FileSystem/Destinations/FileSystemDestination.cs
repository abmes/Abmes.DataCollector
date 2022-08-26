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
        private readonly IHttpClientFactory _httpClientFactory;

        public DestinationConfig DestinationConfig { get; }

        public FileSystemDestination(
            DestinationConfig destinationConfig,
            IFileSystemCommonStorage FileSystemCommonStorage,
            IHttpClientFactory httpClientFactory)
        {
            DestinationConfig = destinationConfig;
            _fileSystemCommonStorage = FileSystemCommonStorage;
            _httpClientFactory = httpClientFactory;
        }

        public bool CanGarbageCollect()
        {
            return true;
        }

        public async Task CollectAsync(string collectUrl, IEnumerable<KeyValuePair<string, string>> collectHeaders, IIdentityServiceClientInfo collectIdentityServiceClientInfo, string dataCollectionName, string fileName, TimeSpan timeout, bool finishWait, int tryNo, CancellationToken cancellationToken)
        {
            int bufferSize = 1 * 1024 * 1024;

            using var httpClient = _httpClientFactory.CreateClient();
            using (var response = await httpClient.SendAsync(collectUrl, HttpMethod.Get, collectUrl, null, null, collectHeaders, null, timeout, null, HttpCompletionOption.ResponseHeadersRead, cancellationToken))
            {
                var sourceMD5 = response.ContentMD5();

                var fullFileName = GetFullFileName(dataCollectionName, fileName);

                Directory.CreateDirectory(Path.GetDirectoryName(fullFileName));

                using (var sourceStream = await response.Content.ReadAsStreamAsync())
                {
                    using (var fileStream = new FileStream(fullFileName, FileMode.Create))
                    {
                        await ParallelCopy.CopyAsync(
                                (buffer, ct) => async () => await CopyUtils.ReadStreamMaxBufferAsync(buffer, sourceStream, ct),
                                (buffer, ct) => async () => await fileStream.WriteAsync(buffer, ct),
                                bufferSize,
                                cancellationToken
                            );
                    }
                }

                using (var fileStream = new FileStream(fullFileName, FileMode.Open, FileAccess.Read))
                {
                    var newMD5 = await CopyUtils.GetMD5HashStringAsync(fileStream, bufferSize, cancellationToken);

                    if ((!string.IsNullOrEmpty(sourceMD5)) &&
                        (!string.Equals(newMD5, sourceMD5, StringComparison.InvariantCultureIgnoreCase)))
                    {
                        throw new Exception("Invalid destination MD5");
                    }

                    File.WriteAllText(GetMD5FileName(fullFileName), newMD5);
                }
            }
        }

        public async Task<IEnumerable<string>> GetDataCollectionFileNamesAsync(string dataCollectionName, CancellationToken cancellationToken)
        {
            return await _fileSystemCommonStorage.GetDataCollectionFileNamesAsync(null, null, DestinationConfig.RootBase(), DestinationConfig.RootDir('\\', false), dataCollectionName, null, cancellationToken);
        }

        private bool IsDirectoryEmpty(string path)
        {
            return !Directory.EnumerateFileSystemEntries(path).Any();
        }

        public async Task GarbageCollectDataCollectionFileAsync(string dataCollectionName, string fileName, CancellationToken cancellationToken)
        {
            var fullFileName = GetFullFileName(dataCollectionName, fileName);

            File.Delete(fullFileName);
            File.Delete(GetMD5FileName(fullFileName));

            var fullDirName = Path.GetDirectoryName(fullFileName);
            var dataCollectionRootFullDirName = Path.Combine(DestinationConfig.Root, dataCollectionName).TrimEnd('\\');

            DeleteEmptyDirectories(fullDirName, dataCollectionRootFullDirName);

            await Task.CompletedTask;
        }

        private void DeleteEmptyDirectories(string fullDirName, string rootFullDirName)
        {
            while (fullDirName.TrimEnd('\\') != rootFullDirName)
            {
                if (!IsDirectoryEmpty(fullDirName))
                {
                    break;
                }

                Directory.Delete(fullDirName);

                fullDirName = Path.GetDirectoryName(fullDirName);
            }
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

        public async Task PutFileAsync(string dataCollectionName, string fileName, Stream content, CancellationToken cancellationToken)
        {
            var fullFileName = GetFullFileName(dataCollectionName, fileName);

            Directory.CreateDirectory(Path.GetDirectoryName(fullFileName));

            using (var fileStream = new FileStream(fullFileName, FileMode.Create))
            {
                await content.CopyToAsync(fileStream);
            }
        }
    }
}
