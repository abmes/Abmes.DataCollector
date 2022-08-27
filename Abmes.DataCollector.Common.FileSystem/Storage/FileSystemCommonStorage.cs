using Abmes.DataCollector.Common.Storage;

namespace Abmes.DataCollector.Common.FileSystem.Storage;

public class FileSystemCommonStorage : IFileSystemCommonStorage
{
    private readonly IFileInfoDataFactory _fileInfoFactory;

    public string StorageType => "FileSystem";

    public FileSystemCommonStorage(
        IFileInfoDataFactory fileInfoFactory)
    {
        _fileInfoFactory = fileInfoFactory;
    }

    public async Task<IEnumerable<string>> GetDataCollectionFileNamesAsync(string loginName, string loginSecret, string rootBase, string rootDir, string dataCollectionName, string fileNamePrefix, CancellationToken cancellationToken)
    {
        return
            (await InternalGetDataCollectionFileInfosAsync(loginName, loginSecret, rootBase, rootDir, dataCollectionName, fileNamePrefix, true, cancellationToken))
            .Select(x => x.Name);
    }

    public async Task<IEnumerable<IFileInfoData>> GetDataCollectionFileInfosAsync(string loginName, string loginSecret, string rootBase, string rootDir, string dataCollectionName, string fileNamePrefix, CancellationToken cancellationToken)
    {
        return await InternalGetDataCollectionFileInfosAsync(loginName, loginSecret, rootBase, rootDir, dataCollectionName, fileNamePrefix, false, cancellationToken);
    }

    private async Task<IEnumerable<IFileInfoData>> InternalGetDataCollectionFileInfosAsync(string loginName, string loginSecret, string rootBase, string rootDir, string dataCollectionName, string fileNamePrefix, bool namesOnly, CancellationToken cancellationToken)
    {
        var fullDirName = System.IO.Path.Combine(rootBase, rootDir, dataCollectionName);
        var searchPattern = fileNamePrefix + "*.*";

        var fileNames = 
                System.IO.Directory.GetFiles(fullDirName, searchPattern, System.IO.SearchOption.AllDirectories)
                .Select(x => x.Substring(fullDirName.Length + 1).Replace(@"\","/"))
                .ToList();

        fileNames = fileNames.Where(x => !x.EndsWith(".md5", StringComparison.InvariantCultureIgnoreCase) || fileNames.Contains(x + ".md5", StringComparer.InvariantCultureIgnoreCase)).ToList();

        return await Task.FromResult(fileNames.Select(x => GetFileInfoAsync(x, fullDirName, namesOnly, cancellationToken).Result));
    }

    private string GetMD5FileName(string fileName)
    {
        return fileName + ".md5";
    }

    private async Task<IFileInfoData> GetFileInfoAsync(string relativeFileName, string fullDirName, bool namesOnly, CancellationToken cancellationToken)
    {
        if (namesOnly)
        {
            return await Task.FromResult(_fileInfoFactory(relativeFileName, null, null, null, StorageType));
        }

        var fullFileName = System.IO.Path.Combine(fullDirName, relativeFileName);

        var fileSize = new System.IO.FileInfo(fullFileName).Length;

        var md5 = System.IO.File.Exists(GetMD5FileName(fullFileName)) ? await System.IO.File.ReadAllTextAsync(GetMD5FileName(fullFileName)) : null;

        return await Task.FromResult(_fileInfoFactory(relativeFileName, fileSize, md5, string.Join("/", relativeFileName.Split('/').SkipLast(1)), StorageType));
    }
}
