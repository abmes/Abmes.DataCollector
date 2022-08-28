namespace Abmes.DataCollector.Vault.Configuration;

public record User
(
    string? IdentityUserId,
    IEnumerable<string>? DataCollectionNames
);
