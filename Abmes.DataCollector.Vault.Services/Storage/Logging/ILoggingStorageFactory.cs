using Abmes.DataCollector.Vault.Services.Ports.Storage;

namespace Abmes.DataCollector.Vault.Services.Storage.Logging;

public delegate ILoggingStorage ILoggingStorageFactory(IStorage storage);
