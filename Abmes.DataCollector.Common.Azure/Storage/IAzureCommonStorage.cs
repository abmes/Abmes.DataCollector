using Abmes.DataCollector.Common.Storage;
using Microsoft.WindowsAzure.Storage.Blob;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Abmes.DataCollector.Common.Azure.Storage
{
    public interface IAzureCommonStorage : ICommonStorage
    {
        Task<CloudBlobContainer> GetContainerAsync(string loginName, string loginSecret, string root, bool createIfNotExists, CancellationToken cancellationToken);
    }
}
