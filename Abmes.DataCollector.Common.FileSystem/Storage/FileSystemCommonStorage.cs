using Abmes.DataCollector.Common.Storage;

namespace Abmes.DataCollector.Common.FileSystem.Storage;

public class FileSystemCommonStorage : IFileSystemCommonStorage
{
    public string StorageType => "FileSystem";

    public async Task<IEnumerable<string>> GetDataCollectionFileNamesAsync(
        string? loginName,
        string? loginSecret,
        string rootBase,
        string rootDir,
        string dataCollectionName,
        string? fileNamePrefix,
        CancellationToken cancellationToken)
    {
        return
            (await InternalGetDataCollectionFileInfosAsync(loginName, loginSecret, rootBase, rootDir, dataCollectionName, fileNamePrefix, true, cancellationToken))
            .Select(x => x.Name);
    }

    public async Task<IEnumerable<FileInfoData>> GetDataCollectionFileInfosAsync(
        string? loginName,
        string? loginSecret,
        string rootBase,
        string rootDir,
        string dataCollectionName,
        string? fileNamePrefix,
        CancellationToken cancellationToken)
    {
        return await InternalGetDataCollectionFileInfosAsync(loginName, loginSecret, rootBase, rootDir, dataCollectionName, fileNamePrefix, false, cancellationToken);
    }

    private async Task<IEnumerable<FileInfoData>> InternalGetDataCollectionFileInfosAsync(
        string? loginName,
        string? loginSecret,
        string rootBase,
        string rootDir,
        string dataCollectionName,
        string? fileNamePrefix,
        bool namesOnly,
        CancellationToken cancellationToken)
    {
        var fullDirName = Path.Combine(rootBase, rootDir, dataCollectionName);
        var searchPattern = fileNamePrefix + "*.*";

        var fileNames = 
                Directory.GetFiles(fullDirName, searchPattern, SearchOption.AllDirectories)
                .Select(x => x.Substring(fullDirName.Length + 1).Replace(@"\","/"))
                .ToList();

        fileNames = fileNames.Where(x => !x.EndsWith(".md5", StringComparison.InvariantCultureIgnoreCase) || fileNames.Contains(x + ".md5", StringComparer.InvariantCultureIgnoreCase)).ToList();

        return await Task.FromResult(fileNames.Select(x => GetFileInfoAsync(x, fullDirName, namesOnly, cancellationToken).Result));
    }

    private string GetMD5FileName(string fileName)
    {
        return fileName + ".md5";
    }

    private async Task<FileInfoData> GetFileInfoAsync(string relativeFileName, string fullDirName, bool namesOnly, CancellationToken cancellationToken)
    {
        if (namesOnly)
        {
            return new FileInfoData(relativeFileName, null, null, null, StorageType);
        }

        var fullFileName = Path.Combine(fullDirName, relativeFileName);

        var fileSize = new FileInfo(fullFileName).Length;

        var md5 = File.Exists(GetMD5FileName(fullFileName)) ? await File.ReadAllTextAsync(GetMD5FileName(fullFileName), cancellationToken) : null;

        return new FileInfoData(relativeFileName, fileSize, md5, string.Join("/", relativeFileName.Split('/').SkipLast(1)), StorageType);
    }
}
