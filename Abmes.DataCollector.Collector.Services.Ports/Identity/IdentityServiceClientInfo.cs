namespace Abmes.DataCollector.Collector.Services.Ports.Identity;

public record IdentityServiceClientInfo
(
    string Url,
    string ClientId,
    string ClientSecret,
    string? Scope,
    string UserName,
    string UserPassword
);
