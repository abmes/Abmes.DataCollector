using Abmes.DataCollector.Utils;

namespace Abmes.DataCollector.Collector.Common.Configuration;

public record DataCollectionConfig
{
    private string? _dataCollectionName;
    public string DataCollectionName { get => Ensure.NotNullOrEmpty(_dataCollectionName); init => _dataCollectionName = value; }

    public string? DataGroupName { get; init; }

    public TimeSpan? InitialDelay { get; init; }

    public string? PrepareUrl { get; init; }
    public IEnumerable<KeyValuePair<string, string>> PrepareHeaders { get; init; } = [];
    public string? PrepareHttpMethod { get; init; }
    public string? PrepareFinishedPollUrl { get; init; }
    public IEnumerable<KeyValuePair<string, string>> PrepareFinishedPollHeaders { get; init; } = [];
    public TimeSpan? PrepareFinishedPollInterval { get; init; }
    public TimeSpan? PrepareDuration { get; init; }

    public string? CollectFileIdentifiersUrl { get; init; }
    public IEnumerable<KeyValuePair<string, string>> CollectFileIdentifiersHeaders { get; init; } = [];
    public string? CollectUrl { get; init; }
    public IEnumerable<KeyValuePair<string, string>> CollectHeaders { get; init; } = [];
    public int? CollectParallelFileCount { get; init; }
    public TimeSpan? CollectTimeout { get; init; }
    public string? CollectFinishWait { get; init; }

    public IEnumerable<string> DestinationIds { get; init; } = [];
    public int? ParallelDestinationCount { get; init; }
    public int? MaxFileCount { get; init; }

    public string? LoginName { get; init; }
    public string? LoginSecret { get; init; }

    public string? IdentityServiceUrl { get; init; }
    public string? IdentityServiceClientId { get; init; }
    public string? IdentityServiceClientSecret { get; init; }
    public string? IdentityServiceScope { get; init; }

    public IEnumerable<KeyValuePair<string, string>> Values { get; init; } = [];


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
}