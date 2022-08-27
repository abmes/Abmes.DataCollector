using Abmes.DataCollector.Vault.Storage;

namespace Abmes.DataCollector.Vault.Logging.Storage
{
    public delegate ILoggingStorage ILoggingStorageFactory(IStorage storage);
}
