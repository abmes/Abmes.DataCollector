using Abmes.DataCollector.Utils;

namespace Abmes.DataCollector.Collector.Common.Configuration;

public record DataCollectionConfig
{
    public string? DataCollectionName { get; init; }
    public string? DataGroupName { get; init; }

    public TimeSpan? InitialDelay { get; init; }

    public string? PrepareUrl { get; init; }
    public IEnumerable<KeyValuePair<string, string>> PrepareHeaders { get; init; } = Enumerable.Empty<KeyValuePair<string, string>>();
    public string? PrepareHttpMethod { get; init; }
    public string? PrepareFinishedPollUrl { get; init; }
    public IEnumerable<KeyValuePair<string, string>> PrepareFinishedPollHeaders { get; init; } = Enumerable.Empty<KeyValuePair<string, string>>();
    public TimeSpan? PrepareFinishedPollInterval { get; init; }
    public TimeSpan? PrepareDuration { get; init; }

    public string? CollectFileIdentifiersUrl { get; init; }
    public IEnumerable<KeyValuePair<string, string>> CollectFileIdentifiersHeaders { get; init; } = Enumerable.Empty<KeyValuePair<string, string>>();
    public string? CollectUrl { get; init; }
    public IEnumerable<KeyValuePair<string, string>> CollectHeaders { get; init; } = Enumerable.Empty<KeyValuePair<string, string>>();
    public int? CollectParallelFileCount { get; init; }
    public TimeSpan? CollectTimeout { get; init; }
    public string? CollectFinishWait { get; init; }

    public IEnumerable<string> DestinationIds { get; init; } = Enumerable.Empty<string>();
    public int? ParallelDestinationCount { get; init; }
    public int? MaxFileCount { get; init; }

    public string? LoginName { get; init; }
    public string? LoginSecret { get; init; }

    public string? IdentityServiceUrl { get; init; }
    public string? IdentityServiceClientId { get; init; }
    public string? IdentityServiceClientSecret { get; init; }
    public string? IdentityServiceScope { get; init; }

    public IEnumerable<KeyValuePair<string, string>> Values { get; init; } = Enumerable.Empty<KeyValuePair<string, string>>();


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
}