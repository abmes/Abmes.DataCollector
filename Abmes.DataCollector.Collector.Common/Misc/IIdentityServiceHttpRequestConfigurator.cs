using Abmes.DataCollector.Collector.Common.Configuration;

namespace Abmes.DataCollector.Collector.Common.Misc;

public interface IIdentityServiceHttpRequestConfigurator
{
    void Config(HttpRequestMessage request, string identityServiceAccessToken, CancellationToken cancellationToken);
    Task ConfigAsync(HttpRequestMessage request, IIdentityServiceClientInfo identityServiceClientInfo, CancellationToken cancellationToken);
    Task<string> GetIdentityServiceAccessTokenAsync(IIdentityServiceClientInfo identityServiceClientInfo, CancellationToken cancellationToken);
}
