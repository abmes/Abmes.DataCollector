using Abmes.DataCollector.Collector.Services.Abstractions.AppConfig;

namespace Abmes.DataCollector.Collector.App.Library.Initialization;

public interface IMainService
{
    Task<int> MainAsync(Action<IBootstrapper>? bootstrap, int exitDelaySeconds, CancellationToken cancellationToken);
}
