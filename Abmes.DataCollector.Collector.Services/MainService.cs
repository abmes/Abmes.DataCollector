using Abmes.DataCollector.Collector.Services.AppConfig;
using Abmes.DataCollector.Collector.Services.Contracts;
using Abmes.DataCollector.Collector.Services.Ports.AppConfig;
using Microsoft.Extensions.Logging;

namespace Abmes.DataCollector.Collector.Services;

public class MainService(
    IMainCollector mainCollector,
    IBootstrapper bootstrapper,
    ITimeFilterProvider timeFilterProvider,
    ITimeFilterProcessor timeFilterProcessor,
    ILogger<MainService> logger)
    : IMainService
{
    public async Task<int> MainAsync(CollectorParams? collectorParams, int exitDelaySeconds, CancellationToken cancellationToken)
    {
        try
        {
            if (collectorParams != null)
            {
                bootstrapper.SetConfig(
                    collectorParams.ConfigSetName,
                    collectorParams.DataCollectionNames,
                    collectorParams.CollectorMode,
                    collectorParams.TimeFilter);
            }

            if (timeFilterProcessor.TimeFilterAccepted(timeFilterProvider.GetTimeFilter()))
            {
                await mainCollector.CollectAsync(cancellationToken);
            }

            return DelayedExitCode(0, exitDelaySeconds);
        }
        catch (Exception e)
        {
            logger.LogCritical(e, "{message}", e.Message);
            return DelayedExitCode(1, exitDelaySeconds);
        }
    }

    private int DelayedExitCode(int exitCode, int delaySeconds = 0)
    {
        if (delaySeconds > 0)
        {
            logger.LogInformation("Exitting after {delaySeconds} seconds ...", delaySeconds);
            Task.Delay(TimeSpan.FromSeconds(delaySeconds)).Wait();
        }

#if DEBUG
        Task.Delay(500).Wait();
        System.Console.WriteLine("Press any key to quit...");
        System.Console.ReadKey();
#endif

        return exitCode;
    }
}
