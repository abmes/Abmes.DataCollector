using Abmes.DataCollector.Collector.Data.Common.Identity;
using Abmes.DataCollector.Collector.Services.Ports.Collecting;
using Abmes.DataCollector.Collector.Services.Ports.Identity;
using Abmes.DataCollector.Shared;
using Abmes.Utils;
using Abmes.Utils.ExecutionStrategy;
using Abmes.Utils.Net;
using System.Text;
using System.Text.Json;

namespace Abmes.DataCollector.Collector.Data.Http.Collecting;

public class CollectItemsProvider(
    ICollectUrlExtractor collectUrlsExtractor,
    IIdentityServiceHttpRequestConfigurator identityServiceHttpRequestConfigurator,
    IHttpClientFactory httpClientFactory,
    IAsyncExecutionStrategy<CollectItemsProvider.IGetCollectItemsMarker> getCollectItemsExecutionStrategy,
    IAsyncExecutionStrategy<CollectItemsProvider.IRedirectCollectItemsMarker> redirectCollectItemsExecutionStrategy,
    IEnumerable<ISimpleContentProvider> simpleContentProviders) : ICollectItemsProvider
{
    public interface IGetCollectItemsMarker { }
    public interface IRedirectCollectItemsMarker { }

    private static readonly string[] DefaultNamePropertyNames = ["name", "fileName", "identifier"];
    private static readonly string[] DefaultSizePropertyNames = ["size", "length"];
    private static readonly string[] DefaultMD5PropertyNames = ["md5", "hash", "checksum"];
    private static readonly string[] DefaultGroupIdPropertyNames = ["group", "groupId"];

    public async Task<IEnumerable<CollectItem>> GetCollectItemsAsync(
        string dataCollectionName,
        string? collectFileIdentifiersUrl,
        IEnumerable<KeyValuePair<string, string>> collectFileIdentifiersHeaders,
        string collectUrl,
        IEnumerable<KeyValuePair<string, string>> collectHeaders,
        IdentityServiceClientInfo? identityServiceClientInfo,
        CancellationToken cancellationToken)
    {
        if (string.IsNullOrEmpty(collectFileIdentifiersUrl))
        {
            if (collectUrl.StartsWith('@'))
            {
                using var httpClient = httpClientFactory.CreateClient();
                var jsonResult =
                    await getCollectItemsExecutionStrategy.ExecuteAsync(
                        async (ct) =>
                            await GetSimpleContentProvidersResultAsync(collectUrl[1..], ct) ??
                            await httpClient.GetStringAsync(collectUrl[1..], collectHeaders, null, null, ct),
                        cancellationToken);

                var urlsString = HttpUtils.FixJsonResult(jsonResult);

                if (!string.IsNullOrEmpty(urlsString))
                {
                    var urls = GetStrings(urlsString) ?? urlsString.Split(Environment.NewLine);

                    return urls.Select(url => new CollectItem(null, url));
                }
                else
                {
                    return [];
                }
            }
            else
            {
                return [new(null, collectUrl)];
            }
        }
        else
        {
            var query = collectFileIdentifiersUrl.Split("@");

            var queryFilter = query.Length > 1 ? query.First() : null;

            var selector = query.Last().Split("|");

            var queryUrl = selector.First();

            var queryPropertyNames =
                selector.Length == 2
                ? selector.Last().Split(',', ';')
                : [];

            var queryNamePropertyName = queryPropertyNames.FirstOrDefault();
            var querySizePropertyName = queryPropertyNames.Skip(1).FirstOrDefault();
            var queryMD5PropertyName = queryPropertyNames.Skip(2).FirstOrDefault();
            var queryGroupIdPropertyName = queryPropertyNames.Skip(3).FirstOrDefault();

            var collectFileIdentifiersJson = GetCollectItemsJson(queryUrl, collectFileIdentifiersHeaders, identityServiceClientInfo, cancellationToken).Result;

            if (!string.IsNullOrEmpty(collectFileIdentifiersJson))
            {
                var namePropertyNames = string.IsNullOrEmpty(queryNamePropertyName) ? DefaultNamePropertyNames : [queryNamePropertyName];
                var sizePropertyNames = string.IsNullOrEmpty(querySizePropertyName) ? DefaultSizePropertyNames : [querySizePropertyName];
                var md5PropertyNames = string.IsNullOrEmpty(queryMD5PropertyName) ? DefaultMD5PropertyNames : [queryMD5PropertyName];
                var groupIdPropertyNames = string.IsNullOrEmpty(queryGroupIdPropertyName) ? DefaultGroupIdPropertyNames : [queryGroupIdPropertyName];

                var collectFileInfos = GetCollectFileInfos(collectFileIdentifiersJson, namePropertyNames, sizePropertyNames, md5PropertyNames, groupIdPropertyNames);

                return
                    collectFileInfos
                    .Where(collectFileInfo =>
                        HttpUtils.IsUrl(collectFileInfo.Name) ||
                        string.IsNullOrEmpty(queryFilter) ||
                        FileMaskUtils.FileNameMatchesFilter(collectFileInfo.Name, queryFilter)
                    )
                    .Select(collectFileInfo =>
                        {
                            if (HttpUtils.IsUrl(collectFileInfo.Name))
                            {
                                return new CollectItem(null, collectFileInfo.Name);
                            }
                            else
                            {
                                ArgumentException.ThrowIfNullOrEmpty(collectUrl);
                                return new CollectItem(collectFileInfo, collectUrl.Replace("[filename]", collectFileInfo.Name, StringComparison.InvariantCultureIgnoreCase));
                            }
                        }
                    );
            }
            else
            {
                return [];
            }
        }
    }

    private async Task<string?> GetSimpleContentProvidersResultAsync(string uri, CancellationToken cancellationToken)
    {
        foreach (var simpleContentProvider in simpleContentProviders)
        {
            var contentBytes = await simpleContentProvider.GetContentAsync(uri, cancellationToken);

            if (contentBytes is not null)
            {
                return Encoding.UTF8.GetString(contentBytes);
            }
        }

        return null;
    }

    public async Task<IEnumerable<CollectItem>> GetRedirectedCollectItemsAsync(
        IEnumerable<CollectItem> collectItems,
        string dataCollectionName,
        IEnumerable<KeyValuePair<string, string>> collectHeaders,
        int maxDegreeOfParallelism,
        IdentityServiceClientInfo? identityServiceClientInfo,
        CancellationToken cancellationToken)
    {
        var collectItemsList = collectItems.ToList();

        var redirectableCollectItems = collectItemsList.Where(x => x.CollectUrl.StartsWith('@'));

        var identityServiceAccessToken =
            identityServiceClientInfo is null
            ? null
            : await identityServiceHttpRequestConfigurator.GetIdentityServiceAccessTokenAsync(identityServiceClientInfo, cancellationToken);

        var redirectedCollectItems =
            await RedirectCollectItemsAsync(
                redirectableCollectItems, dataCollectionName, collectHeaders, maxDegreeOfParallelism, identityServiceAccessToken, cancellationToken);

        return
            collectItemsList
                .Where(x => !x.CollectUrl.StartsWith('@'))
                .Concat(redirectedCollectItems);
    }

    private async Task<IEnumerable<CollectItem>> RedirectCollectItemsAsync(
        IEnumerable<CollectItem> collectItems,
        string dataCollectionName,
        IEnumerable<KeyValuePair<string, string>> collectHeaders,
        int maxDegreeOfParallelism,
        string? identityServiceAccessToken,
        CancellationToken cancellationToken)
    {
        return
            await collectItems.ParallelForEachAsync(
                async (collectItem, ct2) =>
                    await redirectCollectItemsExecutionStrategy.ExecuteAsync(
                        async (ct) =>
                        {
                            var urls = collectItem.CollectUrl.TrimStart('@').Split('|').ToList();
                            var preliminaryUrls = urls.SkipLast(1);
                            var lastUrl = urls.Last();

                            using var httpClient = httpClientFactory.CreateClient();
                            await httpClient.SendManyAsync(preliminaryUrls, HttpMethod.Get, null, ct);

                            var url = await collectUrlsExtractor.ExtractCollectUrlAsync(dataCollectionName, collectItem.CollectFileInfo?.Name, lastUrl, collectHeaders, identityServiceAccessToken, ct);

                            return new CollectItem(collectItem.CollectFileInfo, url);
                        },
                        ct2),
                Math.Max(1, maxDegreeOfParallelism),
                cancellationToken);
    }

    private static readonly JsonSerializerOptions _jsonSerializerOptions = new() { PropertyNameCaseInsensitive = true };

    private static IEnumerable<string>? GetStrings(string json)
    {
        try
        {
            return JsonSerializer.Deserialize<IEnumerable<string>>(json, _jsonSerializerOptions);
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
                var name = namePropertyNames.Where(x => file.TryGetProperty(x, out _)).Select(x => file.GetProperty(x).GetString()).Where(x => !string.IsNullOrEmpty(x)).FirstOrDefault();
                var size = sizePropertyNames.Where(x => file.TryGetProperty(x, out _)).Select(x => file.GetProperty(x).GetInt64()).FirstOrDefault();
                var md5 = md5PropertyNames.Where(x => file.TryGetProperty(x, out _)).Select(x => file.GetProperty(x).GetString()).Where(x => !string.IsNullOrEmpty(x)).FirstOrDefault();
                var groupId = groupIdPropertyNames.Where(x => file.TryGetProperty(x, out _)).Select(x => file.GetProperty(x).GetString()).Where(x => !string.IsNullOrEmpty(x)).FirstOrDefault();

                if (!string.IsNullOrEmpty(name))
                {
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

    private async Task<string> GetCollectItemsJson(string url, IEnumerable<KeyValuePair<string, string>> collectFileIdentifiersHeaders, IdentityServiceClientInfo? identityServiceClientInfo, CancellationToken cancellationToken)
    {
        using var httpClient = httpClientFactory.CreateClient();
        return
            await httpClient.GetStringAsync(
                url,
                HttpMethod.Get,
                headers: collectFileIdentifiersHeaders,
                accept: "application/json",
                requestConfiguratorTask: (request, ct) => identityServiceHttpRequestConfigurator.ConfigAsync(request, identityServiceClientInfo, ct),
                cancellationToken: cancellationToken);
    }
}
