using Abmes.DataCollector.Collector.Services.Contracts;

namespace Abmes.DataCollector.Collector.Services.Configuration;

public interface ICollectorModeProvider
{
    CollectorMode GetCollectorMode();
}
