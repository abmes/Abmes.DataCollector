using Abmes.DataCollector.Collector.Common.Configuration;
using Abmes.DataCollector.Collector.Common.Misc;
using Abmes.DataCollector.Common.Storage;
using Abmes.DataCollector.Utils;
using Polly;
using System.Collections.Concurrent;
using System.Text.Json;

namespace Abmes.DataCollector.Collector.Common.Collecting;

public class CollectItemsProvider : ICollectItemsProvider
{
    private readonly ICollectUrlExtractor _collectUrlExtractor;
    private readonly IIdentityServiceHttpRequestConfigurator _identityServiceHttpRequestConfigurator;
    private readonly IHttpClientFactory _httpClientFactory;

    public CollectItemsProvider(
        ICollectUrlExtractor collectUrlsExtractor,
        IIdentityServiceHttpRequestConfigurator identityServiceHttpRequestConfigurator,
        IHttpClientFactory httpClientFactory)
    {
        _collectUrlExtractor = collectUrlsExtractor;
        _identityServiceHttpRequestConfigurator = identityServiceHttpRequestConfigurator;
        _httpClientFactory = httpClientFactory;
    }

    private static readonly string[] DefaultNamePropertyNames = { "name", "fileName", "identifier" };
    private static readonly string[] DefaultSizePropertyNames = { "size", "length" };
    private static readonly string[] DefaultMD5PropertyNames = { "md5", "hash", "checksum" };
    private static readonly string[] DefaultGroupIdPropertyNames = { "group", "groupId" };

    public IEnumerable<(FileInfoData? CollectFileInfo, string CollectUrl)> GetCollectItems(
        string dataCollectionName,
        string? collectFileIdentifiersUrl,
        IEnumerable<KeyValuePair<string, string>> collectFileIdentifiersHeaders,
        string collectUrl,
        IEnumerable<KeyValuePair<string, string>> collectHeaders,
        IdentityServiceClientInfo identityServiceClientInfo,
        CancellationToken cancellationToken)
    {
        if (string.IsNullOrEmpty(collectFileIdentifiersUrl))
        {
            if (collectUrl.StartsWith("@"))
            {
                using var httpClient = _httpClientFactory.CreateClient();
                var jsonResult =
                        Policy
                        .Handle<Exception>()
                        .WaitAndRetry(new[] { TimeSpan.FromSeconds(5) })
                        .Execute(
                            ct => httpClient.GetStringAsync(collectUrl[1..], collectHeaders, null, null, ct).Result, 
                            cancellationToken
                        );

                var urlsString = HttpUtils.FixJsonResult(jsonResult);

                if (!string.IsNullOrEmpty(urlsString))
                {
                    var urls = GetStrings(urlsString) ?? urlsString.Split(Environment.NewLine);

                    foreach (var url in urls)
                    {
                        yield return (null, url);
                    }
                }
            }
            else
            {
                yield return (null, collectUrl);
            }
        }
        else
        {
            var query = collectFileIdentifiersUrl.Split("@");

            var queryFilter = (query.Length > 1 ? query.First() : null);

            var selector = query.Last().Split("|");

            var queryUrl = selector.First();

            var queryPropertyNames = (selector.Length ==  2) ? selector.Last().Split(',', ';') : Enumerable.Empty<string>();

            var queryNamePropertyName = queryPropertyNames.FirstOrDefault();
            var querySizePropertyName = queryPropertyNames.Skip(1).FirstOrDefault();
            var queryMD5PropertyName = queryPropertyNames.Skip(2).FirstOrDefault();
            var queryGroupIdPropertyName = queryPropertyNames.Skip(3).FirstOrDefault();

            var collectFileIdentifiersJson = GetCollectItemsJson(queryUrl, collectFileIdentifiersHeaders, identityServiceClientInfo, cancellationToken).Result;

            if (!string.IsNullOrEmpty(collectFileIdentifiersJson))
            {
                var namePropertyNames = string.IsNullOrEmpty(queryNamePropertyName) ? DefaultNamePropertyNames : new[] { queryNamePropertyName };
                var sizePropertyNames = string.IsNullOrEmpty(querySizePropertyName) ? DefaultSizePropertyNames : new[] { querySizePropertyName };
                var md5PropertyNames = string.IsNullOrEmpty(queryMD5PropertyName) ? DefaultMD5PropertyNames : new[] { queryMD5PropertyName };
                var groupIdPropertyNames = string.IsNullOrEmpty(queryGroupIdPropertyName) ? DefaultGroupIdPropertyNames : new[] { queryGroupIdPropertyName };

                var collectFileInfos = GetCollectFileInfos(collectFileIdentifiersJson, namePropertyNames, sizePropertyNames, md5PropertyNames, groupIdPropertyNames);

                foreach (var collectFileInfo in collectFileInfos)
                {
                    if (HttpUtils.IsUrl(collectFileInfo.Name))
                    {
                        yield return (null, collectFileInfo.Name);
                    }
                    else
                    {
                        ArgumentExceptionExtensions.ThrowIfNullOrEmpty(collectUrl);

                        if ((string.IsNullOrEmpty(queryFilter)) ||
                            (FileMaskUtils.FileNameMatchesFilter(collectFileInfo.Name, queryFilter)))
                        {
                            yield return (collectFileInfo, collectUrl.Replace("[filename]", collectFileInfo.Name, StringComparison.InvariantCultureIgnoreCase));
                        }
                    }
                }
            }
        }
    }

    public async Task<IEnumerable<(FileInfoData? CollectFileInfo, string CollectUrl)>> GetRedirectedCollectItemsAsync(
        IEnumerable<(FileInfoData? CollectFileInfo, string CollectUrl)> collectItems,
        string dataCollectionName,
        IEnumerable<KeyValuePair<string, string>> collectHeaders,
        int maxDegreeOfParallelism,
        IdentityServiceClientInfo identityServiceClientInfo,
        CancellationToken cancellationToken)
    {
        var collectItemsList = collectItems.ToList();

        var redirectableCollectItems = collectItemsList.Where(x => x.CollectUrl.StartsWith('@'));

        var identityServiceAccessToken = await _identityServiceHttpRequestConfigurator.GetIdentityServiceAccessTokenAsync(identityServiceClientInfo, cancellationToken);

        var redirectedCollectItems = await RedirectCollectItemsAsync(redirectableCollectItems, dataCollectionName, collectHeaders, maxDegreeOfParallelism, identityServiceAccessToken, cancellationToken);

        return
            collectItemsList
                .Where(x => !x.CollectUrl.StartsWith('@'))
                .Concat(redirectedCollectItems);
    }

    private async Task<IEnumerable<(FileInfoData? CollectFileInfo, string CollectUrl)>> RedirectCollectItemsAsync(
        IEnumerable<(FileInfoData? CollectFileInfo, string CollectUrl)> collectItems,
        string dataCollectionName,
        IEnumerable<KeyValuePair<string, string>> collectHeaders,
        int maxDegreeOfParallelism,
        string? identityServiceAccessToken,
        CancellationToken cancellationToken)
    {
        var result = new ConcurrentBag<(FileInfoData? CollectFileInfo, string CollectUrl)>();

        await ParallelUtils.ParallelEnumerateAsync(
            collectItems,
            Math.Max(1, maxDegreeOfParallelism),
            async (collectItem, ct) =>
            {
                var urls = collectItem.CollectUrl.TrimStart('@').Split('|').ToList();
                var preliminaryUrls = urls.SkipLast(1);
                var lastUrl = urls.Last();
                using var httpClient = _httpClientFactory.CreateClient();
                await httpClient.SendManyAsync(preliminaryUrls, HttpMethod.Get, null, cancellationToken);
                var url = await _collectUrlExtractor.ExtractCollectUrlAsync(dataCollectionName, collectItem.CollectFileInfo?.Name, lastUrl, collectHeaders, identityServiceAccessToken, ct);
                result.Add((collectItem.CollectFileInfo, url));
            },
            cancellationToken);

        return result;
    }

    private static IEnumerable<string>? GetStrings(string json)
    {
        try
        {
            var options = new JsonSerializerOptions() { PropertyNameCaseInsensitive = true };
            return JsonSerializer.Deserialize<IEnumerable<string>>(json, options);
        }
        catch
        {
            return null;
        }
    }

    private static IEnumerable<FileInfoData> GetCollectFileInfos(
        string collectFileInfosJson,
        IEnumerable<string> namePropertyNames,
        IEnumerable<string> sizePropertyNames,
        IEnumerable<string> md5PropertyNames,
        IEnumerable<string> groupIdPropertyNames)
    {
        var names = GetStrings(collectFileInfosJson);

        if (names is not null)
        {
            foreach (var name in names)
            {
                yield return new FileInfoData(name, null, null, null, null);
            }
        }
        else
        {
            using var files = JsonDocument.Parse(collectFileInfosJson);

            var hasResult = false;
            foreach (var file in files.RootElement.EnumerateArray())
            {
                var name = namePropertyNames.Select(x => file.GetProperty(x).GetString()).Where(x => !string.IsNullOrEmpty(x)).FirstOrDefault();
                var sizestr = sizePropertyNames.Select(x => file.GetProperty(x).GetString()).Where(x => !string.IsNullOrEmpty(x)).FirstOrDefault();
                var md5 = md5PropertyNames.Select(x => file.GetProperty(x).GetString()).Where(x => !string.IsNullOrEmpty(x)).FirstOrDefault();
                var groupId = groupIdPropertyNames.Select(x => file.GetProperty(x).GetString()).Where(x => !string.IsNullOrEmpty(x)).FirstOrDefault();

                if (!string.IsNullOrEmpty(name))
                {
                    var size = long.TryParse(sizestr, out var outsize) ? (long?)outsize : null;
                    groupId = !string.IsNullOrEmpty(groupId) ? groupId : "-";

                    hasResult = true;

                    yield return new FileInfoData(name, size, md5, groupId, null);
                }
            }

            if (!hasResult)
            {
                var displayPropertyNames = string.Join("|", namePropertyNames);
                throw new Exception($"Could not parse collect file identifiers list as JSON array of strings or objects with property '{displayPropertyNames}'");
            }
        }
    }

    private async Task<string> GetCollectItemsJson(string url, IEnumerable<KeyValuePair<string, string>> collectFileIdentifiersHeaders, IdentityServiceClientInfo identityServiceClientInfo, CancellationToken cancellationToken)
    {
        using var httpClient = _httpClientFactory.CreateClient();
        return
            await httpClient.GetStringAsync(
                url,
                HttpMethod.Get,
                headers: collectFileIdentifiersHeaders,
                accept: "application/json",
                requestConfiguratorTask: (request, ct) => _identityServiceHttpRequestConfigurator.ConfigAsync(request, identityServiceClientInfo, ct),
                cancellationToken: cancellationToken);
    }
}
