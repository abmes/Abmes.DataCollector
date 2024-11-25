namespace Abmes.DataCollector.Collector.Services.Ports.Identity;

public interface IIdentityServiceHttpRequestConfigurator
{
    void Config(HttpRequestMessage request, string? identityServiceAccessToken, CancellationToken cancellationToken);
    Task ConfigAsync(HttpRequestMessage request, IdentityServiceClientInfo? identityServiceClientInfo, CancellationToken cancellationToken);
    Task<string> GetIdentityServiceAccessTokenAsync(IdentityServiceClientInfo identityServiceClientInfo, CancellationToken cancellationToken);
}
