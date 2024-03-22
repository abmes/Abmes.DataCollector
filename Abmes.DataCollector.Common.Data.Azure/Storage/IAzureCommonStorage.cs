﻿using Abmes.DataCollector.Common.Data.Storage;
using Azure.Storage.Blobs;

namespace Abmes.DataCollector.Common.Data.Azure.Storage;

public interface IAzureCommonStorage : ICommonStorage
{
    Task<BlobContainerClient> GetContainerAsync(string loginName, string loginSecret, string root, bool createIfNotExists, CancellationToken cancellationToken);
    string GetConnectionString(string loginName, string loginSecret);
}
