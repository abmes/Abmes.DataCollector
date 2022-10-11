using Abmes.DataCollector.Collector.Common.Collecting;
using Abmes.DataCollector.Collector.Common.Configuration;
using Abmes.DataCollector.Collector.Common.Misc;

namespace Abmes.DataCollector.Collector.ConsoleApp.Initialization;

public class MainService : IMainService
{
    private readonly IMainCollector _mainCollector;
    private readonly IBootstrapper _bootstrapper;
    private readonly ITimeFilterProvider _timeFilterProvider;
    private readonly ITimeFilterProcessor _timeFilterProcessor;

    public MainService(
        IMainCollector mainCollector,
        IBootstrapper bootstrapper,
        ITimeFilterProvider timeFilterProvider,
        ITimeFilterProcessor timeFilterProcessor)
    {
        _mainCollector = mainCollector;
        _bootstrapper = bootstrapper;
        _timeFilterProvider = timeFilterProvider;
        _timeFilterProcessor = timeFilterProcessor;
    }

    public async Task<int> MainAsync(Action<IBootstrapper>? bootstrap, CancellationToken cancellationToken)
    {
        try
        {
            bootstrap?.Invoke(_bootstrapper);

            if (_timeFilterProcessor.TimeFilterAccepted(_timeFilterProvider.GetTimeFilter()))
            {
                await _mainCollector.CollectAsync(cancellationToken);
            }

            return DelayedExitCode(0, 5);
        }
        catch (Exception e)
        {
            System.Console.WriteLine(e.Message);
            return DelayedExitCode(1, 5);
        }
    }

    private static int DelayedExitCode(int exitCode, int delaySeconds = 0)
    {
        if (delaySeconds > 0)
        {
            System.Console.WriteLine($"Exitting after {delaySeconds} seconds ...");
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
