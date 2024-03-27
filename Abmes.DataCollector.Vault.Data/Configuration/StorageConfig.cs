using Abmes.DataCollector.Utils;

namespace Abmes.DataCollector.Vault.Data.Configuration;

public record StorageConfig
{
    private string? _storageType;
    public string StorageType { get => Ensure.NotNullOrEmpty(_storageType); init => _storageType = value; }

    public string? LoginName { get; init; }
    public string? LoginSecret { get; init; }
    public string? Root { get; init; }

    public string RootBase()
    {
        return Root?.Split('/', '\\').FirstOrDefault() ?? string.Empty;
    }

    public string RootDir(char separator, bool includeTrailingSeparator)
    {
        if (Root is null)
        {
            return string.Empty;
        }

        var result = string.Join(separator, Root.Split('/', '\\').Skip(1));

        if (includeTrailingSeparator && (!string.IsNullOrEmpty(result)))
        {
            result += separator;
        }

        return result;
    }
}