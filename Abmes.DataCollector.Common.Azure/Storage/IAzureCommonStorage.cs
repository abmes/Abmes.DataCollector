using Abmes.DataCollector.Common.Storage;
using Azure.Storage.Blobs;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Abmes.DataCollector.Common.Azure.Storage
{
    public interface IAzureCommonStorage : ICommonStorage
    {
        Task<BlobContainerClient> GetContainerAsync(string loginName, string loginSecret, string root, bool createIfNotExists, CancellationToken cancellationToken);
        string GetConnectionString(string loginName, string loginSecret);
    }
}
