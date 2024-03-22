using Abmes.DataCollector.Utils;

namespace Abmes.DataCollector.Common.Data.Configuration;

public record CommonAppSettings : ICommonAppSettings
{
    // todo: .net 7 should support record types with non-nullble props for options
    // non-nullble properties can be checked like this: https://stackoverflow.com/questions/64784374/c-sharp-9-records-validation
    public string? ConfigStorageType { get; init; }

    string ICommonAppSettings.ConfigStorageType => Ensure.NotNullOrEmpty(ConfigStorageType);
}
