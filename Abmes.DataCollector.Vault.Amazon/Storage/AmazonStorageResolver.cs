﻿using Abmes.DataCollector.Vault.Configuration;
using Abmes.DataCollector.Vault.Storage;

namespace Abmes.DataCollector.Vault.Amazon.Storage;

public class AmazonStorageResolver : IStorageResolver
{
    private readonly IAmazonStorageFactory _amazonStorageFactory;

    public AmazonStorageResolver(
        IAmazonStorageFactory amazonStorageFactory)
    {
        _amazonStorageFactory = amazonStorageFactory;
    }

    public bool CanResolve(IStorageConfig storageConfig)
    {
        return string.Equals(storageConfig.StorageType, "Amazon");
    }

    public IStorage GetStorage(IStorageConfig storageConfig)
    {
        return _amazonStorageFactory(storageConfig);
    }
}
