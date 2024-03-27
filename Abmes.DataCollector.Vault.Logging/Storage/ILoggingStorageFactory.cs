using Abmes.DataCollector.Vault.Data.Storage;

namespace Abmes.DataCollector.Vault.Logging.Storage;

public delegate ILoggingStorage ILoggingStorageFactory(IStorage storage);
