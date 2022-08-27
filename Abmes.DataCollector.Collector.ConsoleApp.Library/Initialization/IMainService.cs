using Abmes.DataCollector.Collector.Common.Configuration;

namespace Abmes.DataCollector.Collector.ConsoleApp.Initialization;

public interface IMainService
{
    Task<int> MainAsync(CancellationToken cancellationToken, Action<IBootstrapper> bootstrap = null);
}
