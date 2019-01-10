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

        private async Task<IEnumerable<(IStorage Storage, IEnumerable<string> FileNames)>> InternalGetStorageFileNamesAsync(string fileNamePrefix, CancellationToken cancellationToken)
        {
            var storages = await _storageProvider.GetStoragesAsync(cancellationToken);

            var result = new List<(IStorage Storage, IEnumerable<string> FileNames)>();

            foreach (var storage in storages)
            {
                var fileNames = await storage.GetDataCollectionFileNamesAsync(_dataCollectionName, fileNamePrefix, cancellationToken);
                result.Add((storage, fileNames));
            }

            return result;
        }

        private async Task<(IStorage Storage, IEnumerable<string> FileNames)> InternalGetLatestFileNamesAsync(CancellationToken cancellationToken)
        {
            var storageFileNames = await InternalGetStorageFileNamesAsync(null, cancellationToken);

            return
                storageFileNames
                .Where(x => x.FileNames.Any())
                .Select(x =>
                    ( x.Storage,
                      x.FileNames
                          .GroupBy(z => _fileNameProvider.DataCollectionFileNameToDateTime(z))
                          .OrderBy(z => z.Key)
                          .LastOrDefault()
                    )
                )
                .FirstOrDefault();
        }

        public async Task<IEnumerable<string>> GetFileNamesAsync(string prefix, CancellationToken cancellationToken)
        {
            var storageFileNames = await InternalGetStorageFileNamesAsync(prefix, cancellationToken);
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
            var storageFileNames = await InternalGetStorageFileNamesAsync(fileName, cancellationToken);

            var storage = 
                    storageFileNames
                    .Where(x => x.FileNames.Any())
                    .Select(x => x.Storage)
                    .FirstOrDefault();

            return await storage?.GetDataCollectionFileDownloadUrlAsync(_dataCollectionName, fileName, cancellationToken);
        }

        public async Task<IEnumerable<string>> GetDownloadUrlsAsync(string fileNamePrefix, CancellationToken cancellationToken)
        {
            var storageFileNames = await InternalGetStorageFileNamesAsync(fileNamePrefix, cancellationToken);

            var (storage, fileNames) = storageFileNames.Where(x => x.FileNames.Any()).FirstOrDefault();

            var tasks = fileNames.Select(x => storage.GetDataCollectionFileDownloadUrlAsync(_dataCollectionName, x, cancellationToken));
            await Task.WhenAll(tasks);
            return tasks.Select(x => x.Result);
        }

        public async Task<IEnumerable<string>> GetLatestDownloadUrlsAsync(CancellationToken cancellationToken)
        {
            var (storage, fileNames) = await InternalGetLatestFileNamesAsync(cancellationToken);

            var tasks = fileNames.Select(x => storage.GetDataCollectionFileDownloadUrlAsync(_dataCollectionName, x, cancellationToken));
            await Task.WhenAll(tasks);
            return tasks.Select(x => x.Result);
        }
    }
}
