namespace Abmes.DataCollector.Collector.Services.Contracts.AppConfig;

public interface IBootstrapper
{
    string ConfigSetName { get; }
    string DataCollectionNames { get; }
    CollectorMode CollectorMode { get; }
    string TimeFilter { get; }

    void SetConfig(string? configSetName, string? dataCollectionNames, string? collectorMode, string? timeFilter);
}
