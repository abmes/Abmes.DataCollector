using Abmes.DataCollector.Utils;
using Abmes.DataCollector.Vault.Services.Ports.Configuration;
using Microsoft.Extensions.Logging;

namespace Abmes.DataCollector.Vault.Services.Configuration.Logging;

public class DataCollectionNameProviderLoggingDecorator(
    ILogger<DataCollectionNameProviderLoggingDecorator> logger,
    IDataCollectionNameProvider dataCollectionNameProvider) : IDataCollectionNameProvider
{
    public string GetDataCollectionName()
    {
        try
        {
            logger.LogInformation("Started getting data name");

            var result = dataCollectionNameProvider.GetDataCollectionName();

            logger.LogInformation("Finished getting data name");

            return result;
        }
        catch (Exception e)
        {
            logger.LogCritical("Error getting config data name: {errorMessage}", e.GetAggregateMessages());
            throw;
        }
    }
}
