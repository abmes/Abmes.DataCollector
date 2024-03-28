using Abmes.DataCollector.Utils;

namespace Abmes.DataCollector.Vault.Services.Configuration;

public record User
{
    // todo: c# 11 should have a better way of doing these nullable gymnastics for json sourced records ("field" and "required" keywords?)
    private string? _identityUserId;
    public string IdentityUserId { get => Ensure.NotNullOrEmpty(_identityUserId); init => _identityUserId = value; }

    public IEnumerable<string> DataCollectionNames { get; init; } = [];
}
