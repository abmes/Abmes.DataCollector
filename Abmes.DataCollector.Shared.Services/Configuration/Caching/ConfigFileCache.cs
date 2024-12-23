﻿using Abmes.DataCollector.Shared.Services.Ports.Configuration;
using System.Collections.Concurrent;

namespace Abmes.DataCollector.Shared.Services.Configuration.Caching;

public class ConfigFileCache : IConfigFileCache
{
    private readonly ConcurrentDictionary<string, string> _cache = new();

    public async Task<string> GetConfigFileContentAsync(string fileName, IConfigProvider configFileProvider, CancellationToken cancellationToken)
    {
        var result = _cache.GetOrAdd(fileName, (key) => configFileProvider.GetConfigContentAsync(key, cancellationToken).Result);
        return await Task.FromResult(result);
    }
}
