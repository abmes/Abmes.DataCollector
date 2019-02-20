using Abmes.DataCollector.Vault.Configuration;
using System;
using System.Collections.Generic;
using System.Text;

namespace Abmes.DataCollector.Vault.Storage
{
    public interface IStorageResolverProvider
    {
        IStorageResolver GetResolver(StorageConfig storageConfig);
    }
}
