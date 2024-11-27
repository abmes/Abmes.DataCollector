using Abmes.DataCollector.Collector.Services.Contracts;
using Abmes.DataCollector.Collector.Services.Ports.Configuration;
using Abmes.DataCollector.Utils;
using Microsoft.Extensions.Logging;

namespace Abmes.DataCollector.Collector.Services.Collecting.Logging;

public class MainCollectorLoggingDecorator(
    ILogger<MainCollectorLoggingDecorator> logger,
    IMainCollector collector,
    IConfigSetNameProvider configSetNameProvider,
    ICollectorModeProvider collectorModeProvider) : IMainCollector
{
    private static string ResultPrefix(bool result)
    {
        return
            result
            ? "Finished"
            : "ERRORS occured when";
    }

    public async Task<IEnumerable<string>> CollectAsync(CancellationToken cancellationToken)
    {
        var configSetName = configSetNameProvider.GetConfigSetName();
        var collectorMode = collectorModeProvider.GetCollectorMode();

        var mode = collectorMode.ToString().ToLowerInvariant();

        try
        {
            IEnumerable<string> result;

            logger.LogInformation("[{configSetName}] Started {mode}ing data collections.", configSetName, mode);

            var watch = System.Diagnostics.Stopwatch.StartNew();
            try
            {
                result = await collector.CollectAsync(cancellationToken);
            }
            finally
            {
                watch.Stop();
            }

            var failedDataCollectionNames =
                result.Any()
                ? $" ({string.Join(",", result)})"
                : null;

            logger.LogInformation(
                "[{configSetName}] {resultPrefix} {mode}ing data collections{failedDataCollectionNames}. Elapsed time: {elapsed}",
                configSetName, ResultPrefix(!result.Any()), mode, failedDataCollectionNames, watch.Elapsed);

            return result;
        }
        catch (Exception e)
        {
            logger.LogCritical(
                "[{configSetName}] {resultPrefix} {mode}ing data collections: {errorMessage}",
                configSetName, ResultPrefix(false), mode, e.GetAggregateMessages());

            throw;
        }
    }
}
