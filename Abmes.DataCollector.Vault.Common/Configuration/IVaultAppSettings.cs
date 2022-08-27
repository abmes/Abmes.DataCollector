namespace Abmes.DataCollector.Vault.Configuration;

public interface IVaultAppSettings
{
    TimeSpan DownloadUrlExpiry { get; set; }
}
