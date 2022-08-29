using Abmes.DataCollector.Utils;
using System.Text.Json.Serialization;

namespace Abmes.DataCollector.Collector.Common.Configuration;

[JsonConverter(typeof(JsonStringEnumConverter))]
public enum GarbageCollectionMode { None, Waterfall, Excess }

public record DestinationConfig
(
    string? DestinationId,
    string? DestinationType,
    string? LoginName,
    string? LoginSecret,
    string? Root,

    bool OverrideFiles,
    bool CollectToDirectories,
    bool GenerateFileNames,

    GarbageCollectionMode? GarbageCollectionMode,

    string? CollectPostEndpoint,
    string? GarbageCollectFilePostEndpoint,
    string? FileNamesGetEndpoint,

    string? IdentityServiceUrl,
    string? IdentityServiceClientId,
    string? IdentityServiceClientSecret,
    string? IdentityServiceScope
)
{
    private IdentityServiceClientInfo? _identityServiceClientInfo;
    public IdentityServiceClientInfo IdentityServiceClientInfo
    {
        get
        {
            _identityServiceClientInfo ??=
                new IdentityServiceClientInfo(
                    Url: Ensure.NotNullOrEmpty(IdentityServiceUrl),
                    ClientId: Ensure.NotNullOrEmpty(IdentityServiceClientId),
                    ClientSecret: Ensure.NotNullOrEmpty(IdentityServiceClientSecret),
                    Scope: IdentityServiceScope,
                    UserName: Ensure.NotNullOrEmpty(LoginName),
                    UserPassword: Ensure.NotNullOrEmpty(LoginSecret)
                );

            return _identityServiceClientInfo;
        }
    }

    public string? RootBase()
    {
        return Root?.Split('/', '\\').FirstOrDefault();
    }

    public string? RootDir(char separator, bool includeTrailingSeparator)
    {
        if (Root is null)
        {
            return null;
        }

        var result = string.Join(separator, Root.Split('/', '\\').Skip(1));

        if (includeTrailingSeparator && (!string.IsNullOrEmpty(result)))
        {
            result += separator;
        }

        return result;
    }
}