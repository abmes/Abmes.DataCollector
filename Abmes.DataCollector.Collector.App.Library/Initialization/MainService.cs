using Abmes.DataCollector.Collector.Services.Abstractions.Collecting;
using Abmes.DataCollector.Collector.Services.Abstractions.Configuration;
using Abmes.DataCollector.Collector.Services.Abstractions.Misc;
using Abmes.DataCollector.Collector.Services.Configuration;
using Microsoft.Extensions.Logging;

namespace Abmes.DataCollector.Collector.App.Library.Initialization;

public class MainService(
    IMainCollector mainCollector,
    IBootstrapper bootstrapper,
    ITimeFilterProvider timeFilterProvider,
    ITimeFilterProcessor timeFilterProcessor,
    ILogger<MainService> logger) : IMainService
{
    public async Task<int> MainAsync(Action<IBootstrapper>? bootstrap, int exitDelaySeconds, CancellationToken cancellationToken)
    {
        try
        {
            bootstrap?.Invoke(bootstrapper);

            if (timeFilterProcessor.TimeFilterAccepted(timeFilterProvider.GetTimeFilter()))
            {
                await mainCollector.CollectAsync(cancellationToken);
            }

            return DelayedExitCode(0, exitDelaySeconds);
        }
        catch (Exception e)
        {
            logger.LogCritical(e, e.Message);
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
