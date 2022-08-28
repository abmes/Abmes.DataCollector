using Abmes.DataCollector.Common.Amazon.Storage;
using Abmes.DataCollector.Common.Storage;
using Abmes.DataCollector.Vault.Configuration;
using Amazon.S3;
using Amazon.S3.Model;

namespace Abmes.DataCollector.Vault.Amazon.Storage;

public class AmazonStorage : IAmazonStorage
{
    private readonly IVaultAppSettings _vaultAppSettings;
    private readonly IAmazonS3 _amazonS3;
    private readonly IAmazonCommonStorage _amazonCommonStorage;

    public IStorageConfig StorageConfig { get; }

    public AmazonStorage(
        IStorageConfig storageConfig,
        IVaultAppSettings vaultAppSettings,
        IAmazonS3 amazonS3,
        IAmazonCommonStorage amazonCommonStorage) 
    {
        StorageConfig = storageConfig;
        _vaultAppSettings = vaultAppSettings;
        _amazonS3 = amazonS3;
        _amazonCommonStorage = amazonCommonStorage;
    }

    public async Task<string> GetDataCollectionFileDownloadUrlAsync(string dataCollectionName, string fileName, CancellationToken cancellationToken)
    {
        var request = new GetPreSignedUrlRequest
        {
            BucketName = StorageConfig.RootBase(),
            Key = StorageConfig.RootDir('/', true) + dataCollectionName + "/" + fileName,
            Verb = HttpVerb.GET,
            Expires = DateTime.UtcNow.Add(_vaultAppSettings.DownloadUrlExpiry)
        };

        string result = _amazonS3.GetPreSignedURL(request);

        return await Task.FromResult(result);
    }

    public async Task<IEnumerable<string>> GetDataCollectionFileNamesAsync(string dataCollectionName, string fileNamePrefix, CancellationToken cancellationToken)
    {
        return await _amazonCommonStorage.GetDataCollectionFileNamesAsync(StorageConfig.LoginName, StorageConfig.LoginSecret, StorageConfig.RootBase(), StorageConfig.RootDir('/', true), dataCollectionName, fileNamePrefix, cancellationToken);
    }

    public async Task<IEnumerable<IFileInfoData>> GetDataCollectionFileInfosAsync(string dataCollectionName, string fileNamePrefix, CancellationToken cancellationToken)
    {
        return await _amazonCommonStorage.GetDataCollectionFileInfosAsync(StorageConfig.LoginName, StorageConfig.LoginSecret, StorageConfig.RootBase(), StorageConfig.RootDir('/', true), dataCollectionName, fileNamePrefix, cancellationToken);
    }
}
