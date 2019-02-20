using Abmes.DataCollector.Vault.Configuration;
using System;
using System.Collections.Generic;
using System.Text;

namespace Abmes.DataCollector.Vault.Azure.Storage
{
    public delegate IAzureStorage IAzureStorageFactory(StorageConfig storageConfig);
}
