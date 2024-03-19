using Abmes.DataCollector.Common.Storage;
using Abmes.DataCollector.Vault.Configuration;
using Abmes.DataCollector.Vault.Storage;

namespace Abmes.DataCollector.Vault.Services;

public class DataCollectionFiles(
    IDataCollectionNameProvider dataCollectionNameProvider,
    IStoragesProvider storageProvider,
    IFileNameProvider fileNameProvider) : IDataCollectionFiles
{
    private readonly string _dataCollectionName = dataCollectionNameProvider.GetDataCollectionName();

    private bool FileHasMaxAge(string fileName, TimeSpan? maxAge)
    {
        return
            (!maxAge.HasValue) ||
            string.IsNullOrEmpty(fileName) ||
            (fileNameProvider.DataCollectionFileNameToDateTime(fileName).Add(maxAge.Value) > DateTimeOffset.UtcNow);
    }

    private async Task<IEnumerable<(IStorage Storage, IEnumerable<FileInfoData> FileInfos)>> InternalGetStorageFileInfosAsync(string? fileNamePrefix, TimeSpan? maxAge, CancellationToken cancellationToken)
    {
        var result =
            await GetStorageItemsAsync<FileInfoData>(
                null,
                async storage => GetUnlockedStorageItems((await storage.GetDataCollectionFileInfosAsync(_dataCollectionName, fileNamePrefix, cancellationToken)).Where(x => FileHasMaxAge(x.Name, maxAge)), (x) => x.Name),
                cancellationToken
            );

        return result.Select(x => (x.Storage, FileInfos: x.Items));
    }

    private async Task<IEnumerable<(IStorage Storage, IEnumerable<string> FileNames)>> InternalGetStorageFileNamesAsync(string? fileNamePrefix, string? storageType, TimeSpan? maxAge, CancellationToken cancellationToken)
    {
        return
            await GetStorageItemsAsync(
                storageType,
                async storage => GetUnlockedStorageItems((await storage.GetDataCollectionFileNamesAsync(_dataCollectionName, fileNamePrefix, cancellationToken)).Where(x => FileHasMaxAge(x, maxAge)), (x) => x),
                cancellationToken
            );
    }

    private async Task<IEnumerable<(IStorage Storage, IEnumerable<T> Items)>> GetStorageItemsAsync<T>(string? storageType, Func<IStorage, Task<IEnumerable<T>>> getItemsFunc, CancellationToken cancellationToken)
    {
        var storages = 
            (await storageProvider.GetStoragesAsync(cancellationToken))
            .Where(x => string.IsNullOrEmpty(storageType) || string.Equals(x.StorageConfig.StorageType, storageType, StringComparison.InvariantCultureIgnoreCase));

        var result = new List<(IStorage Storage, IEnumerable<T> Items)>();

        foreach (var storage in storages)
        {
            var items = await getItemsFunc(storage);
            result.Add((storage, items));
        }

        return result;
    }

    private IEnumerable<T> GetUnlockedStorageItems<T>(IEnumerable<T> items, Func<T, string> getItemFileNameFunc)
    {
        items = items.ToList().AsEnumerable();

        var lockFilePrefixes =
                items
                .Select(x => getItemFileNameFunc(x))
                .Where(x => !string.IsNullOrEmpty(x))
                .Where(x => x.EndsWith(fileNameProvider.LockFileName))
                .Select(x => x[..^fileNameProvider.LockFileName.Length])
                .ToList();

        return
            items
            .Select(x => new { Item = x, FileName = getItemFileNameFunc(x) })
            .Where(x => !lockFilePrefixes.Any(y => string.IsNullOrEmpty(y) || x.FileName.StartsWith(y)))
            .Select(x => x.Item);
    }

    private async Task<(IStorage Storage, IEnumerable<string> FileNames)> InternalGetLatestFileNamesAsync(string? storageType, CancellationToken cancellationToken)
    {
        var storageFileNames = await InternalGetStorageFileNamesAsync(null, storageType, null, cancellationToken);

        return await InternalGetLatestItemsAsync(storageFileNames, x => fileNameProvider.DataCollectionFileNameToDateTime(x), cancellationToken);
    }

    private async Task<(IStorage Storage, IEnumerable<FileInfoData> FileInfos)> InternalGetLatestFileInfosAsync(CancellationToken cancellationToken)
    {
        var storageFileInfos = await InternalGetStorageFileInfosAsync(null, null, cancellationToken);

        return await InternalGetLatestItemsAsync(storageFileInfos, x => fileNameProvider.DataCollectionFileNameToDateTime(x.Name), cancellationToken);
    }

    private static async Task<(IStorage Storage, IEnumerable<T> Items)> InternalGetLatestItemsAsync<T>(
        IEnumerable<(IStorage Storage, IEnumerable<T> Items)> storageItems,
        Func<T, DateTimeOffset> getItemDateTimeFunc,
        CancellationToken cancellationToken)
    {
        return await
            Task.FromResult(
                storageItems
                    .Where(x => x.Items.Any())
                    .Select(x =>
                        (
                            x.Storage,
                            x.Items
                                .GroupBy(z => getItemDateTimeFunc(z))
                                .OrderBy(z => z.Key)
                                .LastOrDefault(Enumerable.Empty<T>())
                        )
                    )
                    .FirstOrDefault());
    }

    public async Task<IEnumerable<FileInfoData>> GetFileInfosAsync(string? prefix, TimeSpan? maxAge, CancellationToken cancellationToken)
    {
        var storageFileInfos = await InternalGetStorageFileInfosAsync(prefix, maxAge, cancellationToken);
        return
            storageFileInfos
                .Where(x => x.FileInfos.Any())
                .Select(x => x.FileInfos)
                .FirstOrDefault([]);
    }

    public async Task<IEnumerable<FileInfoData>> GetLatestFileInfosAsync(CancellationToken cancellationToken)
    {
        return (await InternalGetLatestFileInfosAsync(cancellationToken)).FileInfos;
    }

    public async Task<IEnumerable<string>> GetFileNamesAsync(string? prefix, TimeSpan? maxAge, CancellationToken cancellationToken)
    {
        var storageFileNames = await InternalGetStorageFileNamesAsync(prefix, null, maxAge, cancellationToken);
        return 
            storageFileNames
                .Where(x => x.FileNames.Any())
                .Select(x => x.FileNames)
                .FirstOrDefault([]);
    }

    public async Task<IEnumerable<string>> GetLatestFileNamesAsync(CancellationToken cancellationToken)
    {
        return (await InternalGetLatestFileNamesAsync(null, cancellationToken)).FileNames;
    }

    public async Task<string> GetDownloadUrlAsync(string fileName, string? storageType, CancellationToken cancellationToken)
    {
        var storageFileNames = await InternalGetStorageFileNamesAsync(fileName, storageType, null, cancellationToken);

        var storage = 
            storageFileNames
                .Where(x => x.FileNames.Any())
                .Select(x => x.Storage)
                .FirstOrDefault();

        ArgumentNullException.ThrowIfNull(storage);

        return await storage.GetDataCollectionFileDownloadUrlAsync(_dataCollectionName, fileName, cancellationToken);
    }

    public async Task<IEnumerable<string>> GetDownloadUrlsAsync(string? fileNamePrefix, string? storageType, CancellationToken cancellationToken)
    {
        var storageFileNames = await InternalGetStorageFileNamesAsync(fileNamePrefix, storageType,  null, cancellationToken);

        var (storage, fileNames) = storageFileNames.Where(x => x.FileNames.Any()).FirstOrDefault();

        var tasks = fileNames.Select(x => storage.GetDataCollectionFileDownloadUrlAsync(_dataCollectionName, x, cancellationToken)).ToList();
        await Task.WhenAll(tasks);
        return tasks.Select(x => x.Result);
    }

    public async Task<IEnumerable<string>> GetLatestDownloadUrlsAsync(string? storageType, CancellationToken cancellationToken)
    {
        var (storage, fileNames) = await InternalGetLatestFileNamesAsync(storageType, cancellationToken);

        var tasks = fileNames.Select(x => storage.GetDataCollectionFileDownloadUrlAsync(_dataCollectionName, x, cancellationToken)).ToList();
        await Task.WhenAll(tasks);
        return tasks.Select(x => x.Result);
    }
}
