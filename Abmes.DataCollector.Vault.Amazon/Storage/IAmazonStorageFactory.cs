using Abmes.DataCollector.Collector.Common.Configuration;
using Abmes.DataCollector.Vault.Configuration;
using System;
using System.Collections.Generic;
using System.Text;

namespace Abmes.DataCollector.Vault.Amazon.Storage
{
    public delegate IAmazonStorage IAmazonStorageFactory(StorageConfig storageConfig);
}
