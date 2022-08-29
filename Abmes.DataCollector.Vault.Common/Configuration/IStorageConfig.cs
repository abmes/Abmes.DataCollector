namespace Abmes.DataCollector.Vault.Configuration;

public interface IStorageConfig
{
    public string StorageType { get; }
    public string? LoginName { get; }
    public string? LoginSecret { get; }
    public string? Root { get; }

    public string RootBase();
    public string RootDir(char separator, bool includeTrailingSeparator);
}