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

        public async Task<IEnumerable<string>> GetFileNamesAsync(CancellationToken cancellationToken)
        {
            var storages = await _storageProvider.GetStoragesAsync(cancellationToken);

            var results = await Task.WhenAll(storages.Select(x => x.GetDataCollectionFileNamesAsync(_dataCollectionName, cancellationToken)));
            return results.SelectMany(x => x).Distinct();
        }

        public async Task<List<string>> GetLatestFileNamesAsync(CancellationToken cancellationToken)
        {
            var fileNames = await GetFileNamesAsync(cancellationToken);
            return fileNames.GroupBy(x => _fileNameProvider.DataCollectionFileNameToDateTime(x)).OrderBy(x => x.Key).LastOrDefault().Select(x => x).ToList();
        }

        public async Task<string> GetDownloadUrlAsync(string fileName, CancellationToken cancellationToken)
        {
            var storages = await _storageProvider.GetStoragesAsync(cancellationToken);

            var results = await Task.WhenAll(storages.Select(x => x.GetDataCollectionFileDownloadUrlAsync(_dataCollectionName, fileName, cancellationToken)));
            return results.FirstOrDefault();
        }

        public async Task<List<string>> GetLatestDownloadUrlsAsync(CancellationToken cancellationToken)
        {
            var latestFileNames = await GetLatestFileNamesAsync(cancellationToken);
            var tasks = latestFileNames.Select(x => GetDownloadUrlAsync(x, cancellationToken));
            await Task.WhenAll(tasks);
            return tasks.Select(x => x.Result).ToList();
        }

    }
}
