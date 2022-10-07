namespace Abmes.DataCollector.Collector.Common.Configuration;

public interface IBootstrapper
{
    string ConfigSetName { get; }
    string DataCollectionNames { get; }
    CollectorMode CollectorMode { get; }
    string TimeFilter { get; }

    void SetConfig(string? configSetName, string? dataCollectionNames, string? collectorMode, string? timeFilter);
}
