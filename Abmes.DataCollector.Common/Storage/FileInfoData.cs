using System.Text.Json.Serialization;

namespace Abmes.DataCollector.Common.Storage;

public record FileInfoData
(
    // todo: properties should not be nullable. they are nullable because of dual usage of the class. Name-only usages should be refactored to different methods returning return strings
    // todo: immutable record, no interface. no factory
    string Name,

    long? Size,

    [property: JsonPropertyName("md5")]
    string? MD5,

    [property: JsonPropertyName("group")]
    string? GroupId,

    [property: JsonPropertyName("storage")]
    string? StorageType
);
