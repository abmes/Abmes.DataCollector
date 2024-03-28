using Abmes.DataCollector.Collector.Common.Identity;
using Abmes.DataCollector.Collector.Data.Configuration;
using Abmes.DataCollector.Common.Data.FileSystem.Storage;
using Abmes.DataCollector.Utils;

namespace Abmes.DataCollector.Collector.Data.FileSystem.Destinations;

public class FileSystemDestination(
    DestinationConfig destinationConfig,
    IFileSystemCommonStorage fileSystemCommonStorage,
    IHttpClientFactory httpClientFactory) : IFileSystemDestination
{
    public DestinationConfig DestinationConfig => destinationConfig;

    public bool CanGarbageCollect()
    {
        return true;
    }

    private static string? ContentMD5(HttpResponseMessage response)
    {
        return CopyUtils.GetMD5HashString(response.Content.Headers.ContentMD5);
    }

    public async Task CollectAsync(
        string collectUrl,
        IEnumerable<KeyValuePair<string, string>> collectHeaders,
        IdentityServiceClientInfo? collectIdentityServiceClientInfo,
        string dataCollectionName,
        string fileName,
        TimeSpan timeout,
        bool finishWait,
        CancellationToken cancellationToken)
    {
        int bufferSize = 1 * 1024 * 1024;

        using var httpClient = httpClientFactory.CreateClient();
        using var response = await httpClient.SendAsync(collectUrl, HttpMethod.Get, collectUrl, null, null, collectHeaders, null, timeout, null, HttpCompletionOption.ResponseHeadersRead, cancellationToken);
        var sourceMD5 = ContentMD5(response);

        var fullFileName = GetFullFileName(dataCollectionName, fileName);

        var directoryName = Path.GetDirectoryName(fullFileName);
        if (!string.IsNullOrEmpty(directoryName))
        {
            Directory.CreateDirectory(directoryName);
        }

        using (var sourceStream = await response.Content.ReadAsStreamAsync(cancellationToken))
        {
            await using var fileStream = new FileStream(fullFileName, FileMode.Create);
            await ParallelCopy.CopyAsync(
                    (buffer, ct) => async () => await CopyUtils.ReadStreamMaxBufferAsync(buffer, sourceStream, ct),
                    (buffer, ct) => async () => await fileStream.WriteAsync(buffer, ct),
                    bufferSize,
                    cancellationToken
                );
        }

        await using var fileStream2 = new FileStream(fullFileName, FileMode.Open, FileAccess.Read);

        var newMD5 = await CopyUtils.GetMD5HashStringAsync(fileStream2, bufferSize, cancellationToken);

        if ((!string.IsNullOrEmpty(sourceMD5)) &&
            (!string.Equals(newMD5, sourceMD5, StringComparison.InvariantCultureIgnoreCase)))
        {
            throw new Exception("Invalid destination MD5");
        }

        File.WriteAllText(GetMD5FileName(fullFileName), newMD5);
    }

    public async Task<IEnumerable<string>> GetDataCollectionFileNamesAsync(string dataCollectionName, CancellationToken cancellationToken)
    {
        return await fileSystemCommonStorage.GetDataCollectionFileNamesAsync(null, null, DestinationConfig.RootBase(), DestinationConfig.RootDir('\\', false), dataCollectionName, null, cancellationToken);
    }

    private static bool IsDirectoryEmpty(string path)
    {
        return !Directory.EnumerateFileSystemEntries(path).Any();
    }

    public async Task GarbageCollectDataCollectionFileAsync(string dataCollectionName, string fileName, CancellationToken cancellationToken)
    {
        var fullFileName = GetFullFileName(dataCollectionName, fileName);

        File.Delete(fullFileName);
        File.Delete(GetMD5FileName(fullFileName));

        var fullDirName = Path.GetDirectoryName(fullFileName);
        var dataCollectionRootFullDirName = Path.Combine(DestinationConfig.Root ?? string.Empty, dataCollectionName).TrimEnd('\\');

        DeleteEmptyDirectories(fullDirName ?? string.Empty, dataCollectionRootFullDirName);

        await Task.CompletedTask;
    }

    private static void DeleteEmptyDirectories(string fullDirName, string rootFullDirName)
    {
        while (!string.IsNullOrEmpty(fullDirName) && (fullDirName.TrimEnd('\\') != rootFullDirName))
        {
            if (!IsDirectoryEmpty(fullDirName))
            {
                break;
            }

            Directory.Delete(fullDirName);

            fullDirName = Path.GetDirectoryName(fullDirName) ?? string.Empty;
        }
    }

    private static string GetMD5FileName(string fileName)
    {
        return fileName + ".md5";
    }

    private string GetFullFileName(string dataCollectionName, string fileName)
    {
        return Path.Combine(DestinationConfig.Root ?? string.Empty, dataCollectionName, fileName.Replace("/", "\\"));
    }

    private static async Task<string?> GetFileMD5Async(string fullFileName, CancellationToken cancellationToken)
    {
        var md5FileName = GetMD5FileName(fullFileName);

        return File.Exists(md5FileName) ? await File.ReadAllTextAsync(md5FileName, cancellationToken) : null;
    }

    public async Task<bool> AcceptsFileAsync(string dataCollectionName, string name, long? size, string? md5, CancellationToken cancellationToken)
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

        var directoryName = Path.GetDirectoryName(fullFileName);
        ArgumentException.ThrowIfNullOrEmpty(directoryName);

        Directory.CreateDirectory(directoryName);

        await using var fileStream = new FileStream(fullFileName, FileMode.Create);
        await content.CopyToAsync(fileStream, cancellationToken);
    }
}
