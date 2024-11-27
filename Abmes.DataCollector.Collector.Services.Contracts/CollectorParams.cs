namespace Abmes.DataCollector.Collector.Services.Contracts;

public record CollectorParams
{
    public string? ConfigSetName { get; init; }
    public string? DataCollectionNames { get; init; }
    public string? CollectorMode { get; init; }
    public string? TimeFilter { get; init; }
}
