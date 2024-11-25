using System.Text.Json.Serialization;

namespace Abmes.DataCollector.Collector.Services.Ports.Destinations;

[JsonConverter(typeof(JsonStringEnumConverter))]
public enum GarbageCollectionMode { None, Waterfall, Excess }
