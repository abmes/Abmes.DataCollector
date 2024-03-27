using System.Text.Json.Serialization;

namespace Abmes.DataCollector.Collector.Data.Configuration;

[JsonConverter(typeof(JsonStringEnumConverter))]
public enum GarbageCollectionMode { None, Waterfall, Excess }
