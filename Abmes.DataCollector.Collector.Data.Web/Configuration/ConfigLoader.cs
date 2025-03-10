﻿using Abmes.DataCollector.Shared.Services.Ports.Configuration;
using Abmes.Utils.Net;

namespace Abmes.DataCollector.Collector.Data.Web.Configuration;

// todo: move it to Collector as it is not used by Vault so it is not common
public class ConfigLoader(
    IHttpClientFactory httpClientFactory)
    : IConfigLoader
{
    private const string HttpsPrefix = "https://";

    public bool CanLoadFromLocation(string location)
    {
        return
            location.StartsWith(HttpsPrefix, StringComparison.InvariantCultureIgnoreCase) &&
            !location.Equals(HttpsPrefix, StringComparison.InvariantCultureIgnoreCase);
    }

    public bool CanLoadFromStorage(string storageType)
    {
        return false;
    }

    public Task<string> GetConfigContentAsync(string configName, CancellationToken cancellationToken)
    {
        throw new NotImplementedException();
    }

    public async Task<string> GetConfigContentAsync(string configName, string location, CancellationToken cancellationToken)
    {
        var bracketPos = location.IndexOf('[');

        var headers =
            bracketPos >= 0
            ? GetHeaders(location[bracketPos..])
            : null;

        var url = location[..bracketPos].TrimEnd('/') + "/" + configName;

        using var httpClient = httpClientFactory.CreateClient();
        return await httpClient.GetStringAsync(url, headers, null, null, cancellationToken);
    }

    private static IEnumerable<KeyValuePair<string, string>> GetHeaders(string headers)
    {
        return HttpUtils.GetHeaders(headers.Trim('[', ']'));
    }
}
