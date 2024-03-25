using Abmes.DataCollector.Collector.Common.Configuration;

namespace Abmes.DataCollector.Collector.App.ConsoleApp.Initialization;

public interface IMainService
{
    Task<int> MainAsync(Action<IBootstrapper>? bootstrap, int exitDelaySeconds, CancellationToken cancellationToken);
}
