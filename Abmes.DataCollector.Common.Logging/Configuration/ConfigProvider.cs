using Abmes.DataCollector.Common.Configuration;
using Abmes.DataCollector.Utils;
using Microsoft.Extensions.Logging;

namespace Abmes.DataCollector.Common.Logging.Configuration;

public class ConfigProvider(
    ILogger<ConfigProvider> logger,
    IConfigProvider configProvider) : IConfigProvider
{
    public async Task<string> GetConfigContentAsync(string fileName, CancellationToken cancellationToken)
    {
        try
        {
            logger.LogInformation("Started getting config file '{fileName}' content", fileName);

            var result = await configProvider.GetConfigContentAsync(fileName, cancellationToken);

            logger.LogInformation("Finished getting config file '{fileName}' content", fileName);

            return result;
        }
        catch (Exception e)
        {
            logger.LogCritical("Error getting config file '{fileName}' content: {errorMessage}", fileName, e.GetAggregateMessages());
            throw;
        }
    }
}
