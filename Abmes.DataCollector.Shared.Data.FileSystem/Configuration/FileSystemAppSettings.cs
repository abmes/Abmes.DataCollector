using Abmes.DataCollector.Utils;

namespace Abmes.DataCollector.Shared.Data.FileSystem.Configuration;

public record FileSystemAppSettings : IFileSystemAppSettings
{
    // todo: .net 7 should support record types with non-nullble props for options
    // non-nullble properties can be checked like this: https://stackoverflow.com/questions/64784374/c-sharp-9-records-validation
    public string? FileSystemConfigStorageRoot { get; init; }

    string IFileSystemAppSettings.FileSystemConfigStorageRoot => Ensure.NotNullOrEmpty(FileSystemConfigStorageRoot);
}
