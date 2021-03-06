﻿using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Abmes.DataCollector.Common.Storage
{
    public interface ICommonStorage
    {
        string StorageType { get; }
        Task<IEnumerable<string>> GetDataCollectionFileNamesAsync(string loginName, string loginSecret, string rootBase, string rootDir, string dataCollectionName, string fileNamePrefix, CancellationToken cancellationToken);
        Task<IEnumerable<IFileInfo>> GetDataCollectionFileInfosAsync(string loginName, string loginSecret, string rootBase, string rootDir, string dataCollectionName, string fileNamePrefix, CancellationToken cancellationToken);
    }
}
