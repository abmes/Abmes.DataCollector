using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Abmes.DataCollector.Common.Storage;

namespace Abmes.DataCollector.Common.FileSystem.Storage
{
    public class FileSystemCommonStorage : IFileSystemCommonStorage
    {
        private readonly IFileInfoFactory _fileInfoFactory;

        public FileSystemCommonStorage(
            IFileInfoFactory fileInfoFactory)
        {
            _fileInfoFactory = fileInfoFactory;
        }

        public async Task<IEnumerable<string>> GetDataCollectionFileNamesAsync(string loginName, string loginSecret, string rootBase, string rootDir, string dataCollectionName, string fileNamePrefix, CancellationToken cancellationToken)
        {
            return
                (await InternalGetDataCollectionFileInfosAsync(loginName, loginSecret, rootBase, rootDir, dataCollectionName, fileNamePrefix, true, cancellationToken))
                .Select(x => x.Name);
        }

        public async Task<IEnumerable<IFileInfo>> GetDataCollectionFileInfosAsync(string loginName, string loginSecret, string rootBase, string rootDir, string dataCollectionName, string fileNamePrefix, CancellationToken cancellationToken)
        {
            return await InternalGetDataCollectionFileInfosAsync(loginName, loginSecret, rootBase, rootDir, dataCollectionName, fileNamePrefix, false, cancellationToken);
        }

        private async Task<IEnumerable<IFileInfo>> InternalGetDataCollectionFileInfosAsync(string loginName, string loginSecret, string rootBase, string rootDir, string dataCollectionName, string fileNamePrefix, bool namesOnly, CancellationToken cancellationToken)
        {
            var fullDirName = System.IO.Path.Combine(rootBase, rootDir, dataCollectionName);
            var searchPattern = fileNamePrefix + "*.*";

            var fileNames = 
                    System.IO.Directory.GetFiles(fullDirName, searchPattern)
                    .Select(x => x.Substring(fullDirName.Length + 1).Replace(@"\","/"))
                    .ToList();

            fileNames = fileNames.Where(x => fileNames.Contains(x + ".md5")).ToList();

            return await Task.FromResult(fileNames.Select(x => GetFileInfoAsync(x, fullDirName, namesOnly, cancellationToken).Result));
        }

        private async Task<IFileInfo> GetFileInfoAsync(string relativeFileName, string fullDirName, bool namesOnly, CancellationToken cancellationToken)
        {
            if (namesOnly)
            {
                return await Task.FromResult(_fileInfoFactory(relativeFileName, null, null));
            }

            var fullFileName = System.IO.Path.Combine(fullDirName, relativeFileName);

            var fileSize = new System.IO.FileInfo(fullFileName).Length;

            var md5 = await System.IO.File.ReadAllTextAsync(fullDirName);

            return await Task.FromResult(_fileInfoFactory(relativeFileName, fileSize, md5));
        }
    }
}
