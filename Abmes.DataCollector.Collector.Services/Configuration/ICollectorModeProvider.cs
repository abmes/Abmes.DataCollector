using Abmes.DataCollector.Collector.Services.Abstractions.Configuration;

namespace Abmes.DataCollector.Collector.Services.Configuration;

public interface ICollectorModeProvider
{
    CollectorMode GetCollectorMode();
}
