﻿namespace Abmes.DataCollector.Collector.Common.Configuration;

public class Bootstrapper : IBootstrapper
{
    public string? ConfigSetName { get; private set; }

    public string? DataCollectionNames { get; private set; }

    public CollectorMode CollectorMode { get; private set; }

    public string? TimeFilter { get; private set; }


    public void SetConfig(string? configSetName, string? dataCollectionNames, string? collectorMode, string? timeFilter)
    {
        ConfigSetName = configSetName ?? "";
        DataCollectionNames = dataCollectionNames ?? "";
        CollectorMode = string.IsNullOrEmpty(collectorMode) ? CollectorMode.Collect : Enum.Parse<CollectorMode>(collectorMode, true);
        TimeFilter = timeFilter ?? "";
    }
}
