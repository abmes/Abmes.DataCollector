﻿namespace Abmes.DataCollector.Collector.Common.Identity;

public interface IIdentityServiceHttpRequestConfigurator
{
    void Config(HttpRequestMessage request, string? identityServiceAccessToken, CancellationToken cancellationToken);
    Task ConfigAsync(HttpRequestMessage request, IdentityServiceClientInfo? identityServiceClientInfo, CancellationToken cancellationToken);
    Task<string> GetIdentityServiceAccessTokenAsync(IdentityServiceClientInfo identityServiceClientInfo, CancellationToken cancellationToken);
}
