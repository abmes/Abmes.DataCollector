using Abmes.DataCollector.Collector.Common.Configuration;
using Abmes.DataCollector.Vault.Configuration;
using Abmes.DataCollector.Vault.Storage;
using System;
using System.Collections.Generic;
using System.Text;

namespace Abmes.DataCollector.Vault.Logging.Storage
{
    public class LoggingStorageResolver : IStorageResolver
    {
        private readonly IStorageResolver _storageResolver;
        private readonly ILoggingStorageFactory _loggingStorageFactory;

        public LoggingStorageResolver(
            IStorageResolver StorageResolver,
            ILoggingStorageFactory loggingStorageFactory)
        {
            _storageResolver = StorageResolver;
            _loggingStorageFactory = loggingStorageFactory;
        }

        public bool CanResolve(StorageConfig storageConfig)
        {
            return _storageResolver.CanResolve(storageConfig);
        }

        public IStorage GetStorage(StorageConfig storageConfig)
        {
            var storage = _storageResolver.GetStorage(storageConfig);
            return _loggingStorageFactory(storage);
        }
    }
}
