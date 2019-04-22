using Abmes.DataCollector.Common;
using Abmes.DataCollector.Common.Storage;
using Abmes.DataCollector.Vault.Configuration;
using Abmes.DataCollector.Vault.Storage;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Abmes.DataCollector.Vault.Services
{
    public class DataCollectionFiles : IDataCollectionFiles
    {
        private readonly string _dataCollectionName;
        private readonly IStoragesProvider _storageProvider;
        private readonly IFileNameProvider _fileNameProvider;

        public DataCollectionFiles(
            IDataCollectionNameProvider dataCollectionNameProvider,
            IStoragesProvider storageProvider,
            IFileNameProvider fileNameProvider)
        {
            _dataCollectionName = dataCollectionNameProvider.GetDataCollectionName();
            _storageProvider = storageProvider;
            _fileNameProvider = fileNameProvider;
        }

        private bool FileHasMaxAge(string fileName, TimeSpan? maxAge)
        {
            return
                (!maxAge.HasValue) ||
                string.IsNullOrEmpty(fileName) ||
                (_fileNameProvider.DataCollectionFileNameToDateTime(fileName).Add(maxAge.Value) > DateTimeOffset.UtcNow);
        }

        private async Task<IEnumerable<(IStorage Storage, IEnumerable<IFileInfo> FileInfos)>> InternalGetStorageFileInfosAsync(string fileNamePrefix, TimeSpan? maxAge, CancellationToken cancellationToken)
        {
            var result =
                    await GetStorageItemsAsync<IFileInfo>(
                        fileNamePrefix,
                        async storage => GetUnlockedStorageItems((await storage.GetDataCollectionFileInfosAsync(_dataCollectionName, fileNamePrefix, cancellationToken)).Where(x => FileHasMaxAge(x.Name, maxAge)), (x) => x.Name),
                        cancellationToken
                    );

            return await Task.FromResult(result.Select(x => (x.Storage, FileInfos: x.Items)));
        }

        private async Task<IEnumerable<(IStorage Storage, IEnumerable<string> FileNames)>> InternalGetStorageFileNamesAsync(string fileNamePrefix, TimeSpan? maxAge, CancellationToken cancellationToken)
        {
            return
                await GetStorageItemsAsync<string>(
                    fileNamePrefix,
                    async storage => GetUnlockedStorageItems((await storage.GetDataCollectionFileNamesAsync(_dataCollectionName, fileNamePrefix, cancellationToken)).Where(x => FileHasMaxAge(x, maxAge)), (x) => x),
                    cancellationToken
                );
        }

        private async Task<IEnumerable<(IStorage Storage, IEnumerable<T> Items)>> GetStorageItemsAsync<T>(string fileNamePrefix, Func<IStorage, Task<IEnumerable<T>>> getItemsFunc, CancellationToken cancellationToken)
        {
            var storages = await _storageProvider.GetStoragesAsync(cancellationToken);

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
                    .Where(x => x.EndsWith(_fileNameProvider.LockFileName))
                    .Select(x => x.Substring(0, x.Length - _fileNameProvider.LockFileName.Length))
                    .ToList();

            return
                items
                .Select(x => new { Item = x, FileName = getItemFileNameFunc(x) })
                .Where(x => !lockFilePrefixes.Any(y => string.IsNullOrEmpty(y) || x.FileName.StartsWith(y)))
                .Select(x => x.Item);
        }

        private async Task<(IStorage Storage, IEnumerable<string> FileNames)> InternalGetLatestFileNamesAsync(CancellationToken cancellationToken)
        {
            var storageFileNames = await InternalGetStorageFileNamesAsync(null, null, cancellationToken);

            return InternalGetLatestItemsAsync(storageFileNames, x => _fileNameProvider.DataCollectionFileNameToDateTime(x), cancellationToken);
        }

        private async Task<(IStorage Storage, IEnumerable<IFileInfo> FileInfos)> InternalGetLatestFileInfosAsync(CancellationToken cancellationToken)
        {
            var storageFileInfos = await InternalGetStorageFileInfosAsync(null, null, cancellationToken);

            return InternalGetLatestItemsAsync(storageFileInfos, x => _fileNameProvider.DataCollectionFileNameToDateTime(x.Name), cancellationToken);
        }

        private (IStorage Storage, IEnumerable<T> Items) InternalGetLatestItemsAsync<T>(IEnumerable<(IStorage Storage, IEnumerable<T> Items)> storageItems, Func<T, DateTimeOffset> getItemDateTimeFunc, CancellationToken cancellationToken)
        {
            return
                storageItems
                .Where(x => x.Items.Any())
                .Select(x =>
                    (x.Storage,
                      x.Items
                          .GroupBy(z => getItemDateTimeFunc(z))
                          .OrderBy(z => z.Key)
                          .LastOrDefault()
                    )
                )
                .FirstOrDefault();
        }

        public async Task<IEnumerable<IFileInfo>> GetFileInfosAsync(string prefix, TimeSpan? maxAge, CancellationToken cancellationToken)
        {
            var storageFileInfos = await InternalGetStorageFileInfosAsync(prefix, maxAge, cancellationToken);
            return
                storageFileInfos
                .Where(x => x.FileInfos.Any())
                .Select(x => x.FileInfos)
                .FirstOrDefault();
        }

        public async Task<IEnumerable<IFileInfo>> GetLatestFileInfosAsync(CancellationToken cancellationToken)
        {
            return (await InternalGetLatestFileInfosAsync(cancellationToken)).FileInfos;
        }

        public async Task<IEnumerable<string>> GetFileNamesAsync(string prefix, TimeSpan? maxAge, CancellationToken cancellationToken)
        {
            var storageFileNames = await InternalGetStorageFileNamesAsync(prefix, maxAge, cancellationToken);
            return 
                storageFileNames
                .Where(x => x.FileNames.Any())
                .Select(x => x.FileNames)
                .FirstOrDefault();
        }

        public async Task<IEnumerable<string>> GetLatestFileNamesAsync(CancellationToken cancellationToken)
        {
            return (await InternalGetLatestFileNamesAsync(cancellationToken)).FileNames;
        }

        public async Task<string> GetDownloadUrlAsync(string fileName, CancellationToken cancellationToken)
        {
            var storageFileNames = await InternalGetStorageFileNamesAsync(fileName, null, cancellationToken);

            var storage = 
                    storageFileNames
                    .Where(x => x.FileNames.Any())
                    .Select(x => x.Storage)
                    .FirstOrDefault();

            return await storage?.GetDataCollectionFileDownloadUrlAsync(_dataCollectionName, fileName, cancellationToken);
        }

        public async Task<IEnumerable<string>> GetDownloadUrlsAsync(string fileNamePrefix, CancellationToken cancellationToken)
        {
            var storageFileNames = await InternalGetStorageFileNamesAsync(fileNamePrefix, null, cancellationToken);

            var (storage, fileNames) = storageFileNames.Where(x => x.FileNames.Any()).FirstOrDefault();

            var tasks = fileNames.Select(x => storage.GetDataCollectionFileDownloadUrlAsync(_dataCollectionName, x, cancellationToken)).ToList();
            await Task.WhenAll(tasks);
            return tasks.Select(x => x.Result);
        }

        public async Task<IEnumerable<string>> GetLatestDownloadUrlsAsync(CancellationToken cancellationToken)
        {
            var (storage, fileNames) = await InternalGetLatestFileNamesAsync(cancellationToken);

            var tasks = fileNames.Select(x => storage.GetDataCollectionFileDownloadUrlAsync(_dataCollectionName, x, cancellationToken)).ToList();
            await Task.WhenAll(tasks);
            return tasks.Select(x => x.Result);
        }
    }
}
