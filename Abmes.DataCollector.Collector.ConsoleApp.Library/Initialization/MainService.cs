using Abmes.DataCollector.Collector.Common.Collecting;
using Abmes.DataCollector.Collector.Common.Configuration;
using Abmes.DataCollector.Collector.Common.Misc;
using Microsoft.Extensions.Logging;

namespace Abmes.DataCollector.Collector.ConsoleApp.Initialization;

public class MainService : IMainService
{
    private readonly IMainCollector _mainCollector;
    private readonly IBootstrapper _bootstrapper;
    private readonly ITimeFilterProvider _timeFilterProvider;
    private readonly ITimeFilterProcessor _timeFilterProcessor;
    private readonly ILogger<MainService> _logger;

    public MainService(
        IMainCollector mainCollector,
        IBootstrapper bootstrapper,
        ITimeFilterProvider timeFilterProvider,
        ITimeFilterProcessor timeFilterProcessor,
        ILogger<MainService> logger)
    {
        _mainCollector = mainCollector;
        _bootstrapper = bootstrapper;
        _timeFilterProvider = timeFilterProvider;
        _timeFilterProcessor = timeFilterProcessor;
        _logger = logger;
    }

    public async Task<int> MainAsync(Action<IBootstrapper>? bootstrap, int exitDelaySeconds, CancellationToken cancellationToken)
    {
        try
        {
            bootstrap?.Invoke(_bootstrapper);

            if (_timeFilterProcessor.TimeFilterAccepted(_timeFilterProvider.GetTimeFilter()))
            {
                await _mainCollector.CollectAsync(cancellationToken);
            }

            return DelayedExitCode(0, exitDelaySeconds);
        }
        catch (Exception e)
        {
            _logger.LogCritical(e, e.Message);
            return DelayedExitCode(1, exitDelaySeconds);
        }
    }

    private int DelayedExitCode(int exitCode, int delaySeconds = 0)
    {
        if (delaySeconds > 0)
        {
            _logger.LogInformation($"Exitting after {delaySeconds} seconds ...");
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
