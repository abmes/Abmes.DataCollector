﻿using Abmes.DataCollector.Utils;

namespace Abmes.DataCollector.Collector.Common.Configuration;

public class Bootstrapper : IBootstrapper
{
    public string ConfigSetName { get; private set; } = string.Empty;

    public string DataCollectionNames { get; private set; } = string.Empty;

    public CollectorMode CollectorMode { get; private set; }

    public string TimeFilter { get; private set; } = string.Empty;

    public void SetConfig(string? configSetName, string? dataCollectionNames, string? collectorMode, string? timeFilter)
    {
        ConfigSetName = configSetName ?? string.Empty;
        DataCollectionNames = dataCollectionNames ?? string.Empty;
        CollectorMode = string.IsNullOrEmpty(collectorMode) ? CollectorMode.Collect : Enum.Parse<CollectorMode>(collectorMode, true);
        TimeFilter = timeFilter ?? string.Empty;
    }
}
