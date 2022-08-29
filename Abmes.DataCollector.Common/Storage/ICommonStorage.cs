namespace Abmes.DataCollector.Common.Storage;

public interface ICommonStorage
{
    string StorageType { get; }
    Task<IEnumerable<string>> GetDataCollectionFileNamesAsync(string? loginName, string? loginSecret, string rootBase, string rootDir, string dataCollectionName, string? fileNamePrefix, CancellationToken cancellationToken);
    Task<IEnumerable<FileInfoData>> GetDataCollectionFileInfosAsync(string? loginName, string? loginSecret, string rootBase, string rootDir, string dataCollectionName, string? fileNamePrefix, CancellationToken cancellationToken);
}
