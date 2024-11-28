using System.Text.Json.Serialization;

namespace Abmes.DataCollector.Collector.Services.Ports.Destinations;

[JsonConverter(typeof(JsonStringEnumConverter))]  // todo: is there another way to do this without taking a dependency on System.Text.Json?
public enum GarbageCollectionMode { None, Waterfall, Excess }
