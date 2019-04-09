using Abmes.DataCollector.Common.Storage;
using Abmes.DataCollector.Vault.Configuration;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Abmes.DataCollector.Vault.Storage
{
    public interface IStorage
    {
        StorageConfig StorageConfig { get; }
        Task<IEnumerable<IFileInfo>> GetDataCollectionFileInfosAsync(string dataCollectionName, string fileNamePrefix, CancellationToken cancellationToken);
        Task<IEnumerable<string>> GetDataCollectionFileNamesAsync(string dataCollectionName, string fileNamePrefix, CancellationToken cancellationToken);
        Task<string> GetDataCollectionFileDownloadUrlAsync(string dataCollectionName, string fileName, CancellationToken cancellationToken);
    }
}
