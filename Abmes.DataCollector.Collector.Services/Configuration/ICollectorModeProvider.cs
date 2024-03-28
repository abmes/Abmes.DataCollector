using Abmes.DataCollector.Collector.Services.Abstractions;

namespace Abmes.DataCollector.Collector.Services.Configuration;

public interface ICollectorModeProvider
{
    CollectorMode GetCollectorMode();
}
