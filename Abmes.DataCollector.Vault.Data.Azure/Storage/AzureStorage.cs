using Abmes.DataCollector.Common;
using Abmes.DataCollector.Common.Data.Azure.Storage;
using Abmes.DataCollector.Vault.Data.Configuration;
using Azure.Storage;
using Azure.Storage.Sas;

namespace Abmes.DataCollector.Vault.Data.Azure.Storage;

public class AzureStorage(
    StorageConfig storageConfig,
    TimeProvider timeProvider,
    IVaultAppSettings vaultAppSettings,
    IAzureCommonStorage azureCommonStorage) : IAzureStorage
{
    public StorageConfig StorageConfig => storageConfig;

    public async Task<string> GetDataCollectionFileDownloadUrlAsync(string dataCollectionName, string fileName, CancellationToken cancellationToken)
    {
        var containerName =
            string.IsNullOrEmpty(StorageConfig.Root)
            ? dataCollectionName
            : StorageConfig.RootBase();

        var blobName = GetBlobName(dataCollectionName, fileName);

        var sasBuilder = new BlobSasBuilder()
        {
            BlobContainerName = containerName,
            BlobName = blobName,
            Resource = "b",
            StartsOn = timeProvider.GetUtcNow().AddMinutes(-1),
            ExpiresOn = timeProvider.GetUtcNow().Add(vaultAppSettings.DownloadUrlExpiry)
        };
        sasBuilder.SetPermissions(BlobSasPermissions.Read);

        var credential = new StorageSharedKeyCredential(StorageConfig.LoginName, StorageConfig.LoginSecret);
        var sasToken = sasBuilder.ToSasQueryParameters(credential).ToString();

        var uriBuilder = new UriBuilder()
        {
            Scheme = "https",
            Host = string.Format("{0}.blob.core.windows.net", StorageConfig.LoginName),
            Path = string.Format("{0}/{1}", containerName, blobName),
            Query = sasToken
        };

        return await Task.FromResult(uriBuilder.Uri.ToString());
    }

    private string GetBlobName(string dataCollectionName, string fileName)
    {
        return
            string.IsNullOrEmpty(StorageConfig.Root)
            ? fileName
            : StorageConfig.RootDir('/', true) + dataCollectionName + "/" + fileName;
    }

    public async Task<IEnumerable<string>> GetDataCollectionFileNamesAsync(string dataCollectionName, string? fileNamePrefix, CancellationToken cancellationToken)
    {
        return await azureCommonStorage.GetDataCollectionFileNamesAsync(StorageConfig.LoginName, StorageConfig.LoginSecret, StorageConfig.RootBase(), StorageConfig.RootDir('/', true), dataCollectionName, fileNamePrefix, cancellationToken);
    }

    public async Task<IEnumerable<FileInfoData>> GetDataCollectionFileInfosAsync(string dataCollectionName, string? fileNamePrefix, CancellationToken cancellationToken)
    {
        return await azureCommonStorage.GetDataCollectionFileInfosAsync(StorageConfig.LoginName, StorageConfig.LoginSecret, StorageConfig.RootBase(), StorageConfig.RootDir('/', true), dataCollectionName, fileNamePrefix, cancellationToken);
    }
}
