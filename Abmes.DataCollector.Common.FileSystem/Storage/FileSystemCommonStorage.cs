using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace Abmes.DataCollector.Common.FileSystem.Storage
{
    public class FileSystemCommonStorage : IFileSystemCommonStorage
    {
        public async Task<IEnumerable<string>> GetDataCollectionFileNamesAsync(string loginName, string loginSecret, string rootBase, string rootDir, string dataCollectionName, string fileNamePrefix, CancellationToken cancellationToken)
        {
            var dirName = System.IO.Path.Combine(rootBase, rootDir, dataCollectionName);
            var searchPattern = fileNamePrefix + "*.*";

            var fileNames = 
                    System.IO.Directory.GetFiles(dirName, searchPattern)
                    .Select(x => x.Substring(dirName.Length + 1).Replace(@"\","/"))
                    .ToList();

            var result = fileNames.Where(x => fileNames.Contains(x + ".md5"));

            return await Task.FromResult(result);
        }
    }
}
