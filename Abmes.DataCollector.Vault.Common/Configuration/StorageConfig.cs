﻿using Abmes.DataCollector.Utils;

namespace Abmes.DataCollector.Vault.Configuration;

public record StorageConfig
(
    string? StorageType,
    string? LoginName,
    string? LoginSecret,
    string? Root
)
: IStorageConfig  // todo: interface not needed
{
    string IStorageConfig.StorageType => Ensure.NotNullOrEmpty(StorageType);

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