using Abmes.DataCollector.Vault.Data.Configuration;
using Microsoft.Extensions.Logging;

namespace Abmes.DataCollector.Vault.Logging.Configuration;

public class StorageConfigProvider(
    ILogger<IStoragesConfigProvider> logger,
    IStoragesConfigProvider destinationsConfigProvider) : IStoragesConfigProvider
{
    public async Task<IEnumerable<StorageConfig>> GetStorageConfigsAsync(CancellationToken cancellationToken)
    {
        logger.LogTrace("Started getting storage config");

        var result = await destinationsConfigProvider.GetStorageConfigsAsync(cancellationToken);

        logger.LogTrace("Finished getting storage config");

        return result;
    }
}
