using Abmes.DataCollector.Collector.Services.Ports.AppConfig;

namespace Abmes.DataCollector.Collector.Services.Ports.Configuration;

public interface ICollectorModeProvider
{
    CollectorMode GetCollectorMode();
}
