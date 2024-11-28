using Abmes.DataCollector.Collector.Services.Ports.Identity;

namespace Abmes.DataCollector.Collector.Data.Common.Identity;

public interface IIdentityServiceHttpRequestConfigurator
{
    void Config(HttpRequestMessage request, string? identityServiceAccessToken, CancellationToken cancellationToken);
    Task ConfigAsync(HttpRequestMessage request, IdentityServiceClientInfo? identityServiceClientInfo, CancellationToken cancellationToken);
    Task<string> GetIdentityServiceAccessTokenAsync(IdentityServiceClientInfo identityServiceClientInfo, CancellationToken cancellationToken);
}
