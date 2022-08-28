using Abmes.DataCollector.Utils;

namespace Abmes.DataCollector.Collector.Common.Configuration;

public record DataCollectionConfig(
    string? DataCollectionName,
    string? DataGroupName,

    TimeSpan? InitialDelay,

    string? PrepareUrl,
    IEnumerable<KeyValuePair<string?, string?>>? PrepareHeaders,
    string? PrepareHttpMethod,
    string? PrepareFinishedPollUrl,
    IEnumerable<KeyValuePair<string?, string?>>? PrepareFinishedPollHeaders,
    TimeSpan? PrepareFinishedPollInterval,
    TimeSpan? PrepareDuration,

    string? CollectFileIdentifiersUrl,
    IEnumerable<KeyValuePair<string?, string?>>? CollectFileIdentifiersHeaders,
    string? CollectUrl,
    IEnumerable<KeyValuePair<string?, string?>>? CollectHeaders,
    int? CollectParallelFileCount,
    TimeSpan? CollectTimeout,
    string? CollectFinishWait,

    IEnumerable<string?>? DestinationIds,
    int? ParallelDestinationCount,
    int? MaxFileCount,

    string? LoginName,
    string? LoginSecret,

    string? IdentityServiceUrl,
    string? IdentityServiceClientId,
    string? IdentityServiceClientSecret,
    string? IdentityServiceScope,

    IEnumerable<KeyValuePair<string?, string?>>? Values
)
{
    private IIdentityServiceClientInfo? _identityServiceClientInfo;
    public IIdentityServiceClientInfo IdentityServiceClientInfo 
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
}