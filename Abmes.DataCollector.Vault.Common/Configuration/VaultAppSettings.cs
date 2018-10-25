using System;

namespace Abmes.DataCollector.Vault.Configuration
{
    public class VaultAppSettings : IVaultAppSettings
    {
        public TimeSpan DownloadUrlExpiry { get; set; }
    }
}
