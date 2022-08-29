using Abmes.DataCollector.Common.Azure.Storage;
using Abmes.DataCollector.Common.Storage;
using Abmes.DataCollector.Vault.Configuration;
using Azure.Storage;
using Azure.Storage.Sas;

namespace Abmes.DataCollector.Vault.Azure.Storage;

public class AzureStorage : IAzureStorage
{
    private readonly IVaultAppSettings _vaultAppSettings;
    private readonly IAzureCommonStorage _azureCommonStorage;

    public StorageConfig StorageConfig { get; }

    public AzureStorage(
        StorageConfig storageConfig,
        IVaultAppSettings vaultAppSettings,
        IAzureCommonStorage azureCommonStorage)
    {
        StorageConfig = storageConfig;
        _vaultAppSettings = vaultAppSettings;
        _azureCommonStorage = azureCommonStorage;
    }

    public async Task<string> GetDataCollectionFileDownloadUrlAsync(string dataCollectionName, string fileName, CancellationToken cancellationToken)
    {
        var containerName = string.IsNullOrEmpty(StorageConfig.Root) ? dataCollectionName : StorageConfig.RootBase();
        var blobName = GetBlobName(dataCollectionName, fileName);

        var sasBuilder = new BlobSasBuilder()
        {
            BlobContainerName = containerName,
            BlobName = blobName,
            Resource = "b",
            StartsOn = DateTimeOffset.UtcNow.AddMinutes(-1),
            ExpiresOn = DateTimeOffset.UtcNow.Add(_vaultAppSettings.DownloadUrlExpiry)
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
        return string.IsNullOrEmpty(StorageConfig.Root) ? fileName : (StorageConfig.RootDir('/', true) + dataCollectionName + "/" + fileName);
    }

    public async Task<IEnumerable<string>> GetDataCollectionFileNamesAsync(string dataCollectionName, string? fileNamePrefix, CancellationToken cancellationToken)
    {
        return await _azureCommonStorage.GetDataCollectionFileNamesAsync(StorageConfig.LoginName, StorageConfig.LoginSecret, StorageConfig.RootBase(), StorageConfig.RootDir('/', true), dataCollectionName, fileNamePrefix, cancellationToken);
    }

    public async Task<IEnumerable<FileInfoData>> GetDataCollectionFileInfosAsync(string dataCollectionName, string? fileNamePrefix, CancellationToken cancellationToken)
    {
        return await _azureCommonStorage.GetDataCollectionFileInfosAsync(StorageConfig.LoginName, StorageConfig.LoginSecret, StorageConfig.RootBase(), StorageConfig.RootDir('/', true), dataCollectionName, fileNamePrefix, cancellationToken);
    }
}
