using Abmes.DataCollector.Collector.Services.Abstractions.Configuration;

namespace Abmes.DataCollector.Collector.App.Library.Initialization;

public interface IMainService
{
    Task<int> MainAsync(Action<IBootstrapper>? bootstrap, int exitDelaySeconds, CancellationToken cancellationToken);
}
