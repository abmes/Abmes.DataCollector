using Abmes.DataCollector.Common.Azure.Configuration;
using Abmes.DataCollector.Common.Storage;
using Abmes.DataCollector.Utils;
using Azure.Storage.Blobs;
using Azure.Storage.Blobs.Models;

namespace Abmes.DataCollector.Common.Azure.Storage;

public class AzureCommonStorage(
    IAzureAppSettings commonAppSettings) : IAzureCommonStorage
{
    public string StorageType => "Azure";

    public async Task<BlobContainerClient> GetContainerAsync(string loginName, string loginSecret, string root, bool createIfNotExists, CancellationToken cancellationToken)
    {
        var connectionString = GetConnectionString(loginName, loginSecret);

        var container = new BlobContainerClient(connectionString, root.Replace('_', '-'));

        if (createIfNotExists)
        {
            await container.CreateIfNotExistsAsync(PublicAccessType.None, null, null, cancellationToken);
        }

        return container;
    }

    public async Task<IEnumerable<string>> GetDataCollectionFileNamesAsync(string? loginName, string? loginSecret, string rootBase, string rootDir, string dataCollectionName, string? fileNamePrefix, CancellationToken cancellationToken)
    {
        ArgumentExceptionExtensions.ThrowIfNullOrEmpty(loginName);
        ArgumentExceptionExtensions.ThrowIfNullOrEmpty(loginSecret);

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
        ArgumentExceptionExtensions.ThrowIfNullOrEmpty(loginName);
        ArgumentExceptionExtensions.ThrowIfNullOrEmpty(loginSecret);

        return await InternalGetDataCollectionFileInfosAsync(loginName, loginSecret, rootBase, rootDir, dataCollectionName, fileNamePrefix, false, cancellationToken);
    }

    public string GetConnectionString(string loginName, string loginSecret)
    {
        return $"DefaultEndpointsProtocol=https;AccountName={loginName};AccountKey={loginSecret};EndpointSuffix=core.windows.net";
    }

    private async Task<IEnumerable<FileInfoData>> InternalGetDataCollectionFileInfosAsync(string loginName, string loginSecret, string rootBase, string rootDir, string dataCollectionName, string? fileNamePrefix, bool namesOnly, CancellationToken cancellationToken)
    {
        var root = string.IsNullOrEmpty(rootBase) ? dataCollectionName : rootBase;
        var container = await GetContainerAsync(loginName, loginSecret, root, false, cancellationToken);

        var containerExists = await container.ExistsAsync(cancellationToken);
        if (!containerExists)
        {
            return Enumerable.Empty<FileInfoData>();
        }

        var prefix = string.IsNullOrEmpty(rootBase) ? null : (rootDir + dataCollectionName + "/");

        var prefixSections = string.IsNullOrEmpty(prefix) ? 0 : (prefix.TrimEnd('/').Split('/').Length);

        var blobTraits = namesOnly ? BlobTraits.None : BlobTraits.Metadata;

        var blobs = container.GetBlobs(blobTraits, BlobStates.None, prefix + fileNamePrefix, cancellationToken);

        return blobs.Select(x => GetFileInfoAsync(x, prefixSections, namesOnly, cancellationToken).Result).ToList();
    }

    private async Task<FileInfoData> GetFileInfoAsync(BlobItem blob, int prefixSections, bool namesOnly, CancellationToken cancellationToken)
    {
        var name = string.Join("/", blob.Name.Split("/", StringSplitOptions.RemoveEmptyEntries).Skip(prefixSections));

        if (namesOnly)
        {
            return await Task.FromResult(new FileInfoData(name, null, null, null, StorageType));
        }

        return new FileInfoData(
            name,
            blob.Properties.ContentLength,
            Convert.ToBase64String(blob.Properties.ContentHash),
            string.Join("/", name.Split('/').SkipLast(1)),
            StorageType);
    }
}
