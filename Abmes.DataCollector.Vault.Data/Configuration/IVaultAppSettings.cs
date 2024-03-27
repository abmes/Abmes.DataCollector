namespace Abmes.DataCollector.Vault.Data.Configuration;

public interface IVaultAppSettings
{
    TimeSpan DownloadUrlExpiry { get; }
}
