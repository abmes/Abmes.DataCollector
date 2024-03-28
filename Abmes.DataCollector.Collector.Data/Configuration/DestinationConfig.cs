using Abmes.DataCollector.Collector.Common.Identity;
using Abmes.DataCollector.Utils;

namespace Abmes.DataCollector.Collector.Data.Configuration;

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
    public IdentityServiceClientInfo? IdentityServiceClientInfo
    {
        get
        {
            _identityServiceClientInfo ??=
                string.IsNullOrEmpty(IdentityServiceUrl)
                ?
                null
                :
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

    public string RootBase()
    {
        return Root?.Split('/', '\\').FirstOrDefault() ?? string.Empty;
    }

    public string RootDir(char separator, bool includeTrailingSeparator)
    {
        if (Root is null)
        {
            return string.Empty;
        }

        var result = string.Join(separator, Root.Split('/', '\\').Skip(1));

        if (includeTrailingSeparator && (!string.IsNullOrEmpty(result)))
        {
            result += separator;
        }

        return result;
    }
}