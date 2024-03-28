using System.Text.Json.Serialization;

namespace Abmes.DataCollector.Common;

public record FileInfoData
(
    string Name,

    long? Size,

    [property: JsonPropertyName("md5")]
    string? MD5,

    [property: JsonPropertyName("group")]
    string? GroupId,

    [property: JsonPropertyName("storage")]
    string? StorageType
);
