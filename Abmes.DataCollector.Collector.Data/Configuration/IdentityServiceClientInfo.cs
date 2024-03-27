namespace Abmes.DataCollector.Collector.Data.Configuration;

public record IdentityServiceClientInfo
(
    string Url,
    string ClientId,
    string ClientSecret,
    string? Scope,
    string UserName,
    string UserPassword
);
