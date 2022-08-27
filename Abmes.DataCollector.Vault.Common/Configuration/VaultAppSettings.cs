using Abmes.DataCollector.Utils;

namespace Abmes.DataCollector.Vault.Configuration;

public record VaultAppSettings : IVaultAppSettings
{
    // todo: .net 7 should support record types with non-nullble props for options
    // non-nullble properties can be checked like this: https://stackoverflow.com/questions/64784374/c-sharp-9-records-validation
    public TimeSpan? DownloadUrlExpiry { get; init; }

    TimeSpan IVaultAppSettings.DownloadUrlExpiry => Ensure.NotNull<TimeSpan>(DownloadUrlExpiry);
}
