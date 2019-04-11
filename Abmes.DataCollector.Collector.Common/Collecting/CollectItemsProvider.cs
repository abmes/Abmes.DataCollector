using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Abmes.DataCollector.Collector.Common.Misc;
using System;
using System.Collections.Generic;
using System.Diagnostics.Contracts;
using System.Linq;
using System.Text;
using Abmes.DataCollector.Utils;
using System.Threading;
using System.Collections.Concurrent;
using System.Threading.Tasks;
using Abmes.DataCollector.Common.Storage;
using Abmes.DataCollector.Collector.Common.Configuration;
using System.Net.Http;

namespace Abmes.DataCollector.Collector.Common.Collecting
{
    public class CollectItemsProvider : ICollectItemsProvider
    {
        private readonly ICollectUrlExtractor _collectUrlExtractor;
        private readonly IFileInfoFactory _fileInfoFactory;
        private readonly IIdentityServiceHttpRequestConfigurator _identityServiceHttpRequestConfigurator;

        public CollectItemsProvider(
            ICollectUrlExtractor collectUrlsExtractor,
            IFileInfoFactory fileInfoFactory,
            IIdentityServiceHttpRequestConfigurator identityServiceHttpRequestConfigurator)
        {
            _collectUrlExtractor = collectUrlsExtractor;
            _fileInfoFactory = fileInfoFactory;
            _identityServiceHttpRequestConfigurator = identityServiceHttpRequestConfigurator;
        }

        private static readonly string[] DefaultNamePropertyNames = { "name", "fileName", "identifier" };
        private static readonly string[] DefaultSizePropertyNames = { "size", "length" };
        private static readonly string[] DefaultMD5PropertyNames = { "md5", "hash", "checksum" };

        public IEnumerable<(IFileInfo CollectFileInfo, string CollectUrl)> GetCollectItems(string dataCollectionName, string collectFileIdentifiersUrl, IEnumerable<KeyValuePair<string, string>> collectFileIdentifiersHeaders, string collectUrl, IIdentityServiceClientInfo identityServiceClientInfo, CancellationToken cancellationToken)
        {
            if (string.IsNullOrEmpty(collectFileIdentifiersUrl))
            {
                yield return (null, collectUrl);
            }
            else
            {
                var query = collectFileIdentifiersUrl.Split("@");

                var queryFilter = (query.Length > 1 ? query.First() : null);

                var selector = query.Last().Split("|");

                var queryUrl = selector.First();

                var queryPropertyNames = (selector.Count() ==  2) ? selector.Last().Split(',', ';') : Enumerable.Empty<string>();

                var queryNamePropertyName = queryPropertyNames.FirstOrDefault();
                var querySizePropertyName = queryPropertyNames.Skip(1).FirstOrDefault();
                var queryMD5PropertyName = queryPropertyNames.Skip(1).FirstOrDefault();

                var collectFileIdentifiersJson = GetCollectItemsJson(queryUrl, collectFileIdentifiersHeaders, identityServiceClientInfo, cancellationToken).Result;

                if (!string.IsNullOrEmpty(collectFileIdentifiersJson))
                {
                    var namePropertyNames = string.IsNullOrEmpty(queryNamePropertyName) ? DefaultNamePropertyNames : new[] { queryNamePropertyName };
                    var sizePropertyNames = string.IsNullOrEmpty(querySizePropertyName) ? DefaultSizePropertyNames : new[] { querySizePropertyName };
                    var md5PropertyNames = string.IsNullOrEmpty(queryMD5PropertyName) ? DefaultMD5PropertyNames : new[] { queryMD5PropertyName };

                    var collectFileInfos = GetCollectFileInfos(collectFileIdentifiersJson, namePropertyNames, sizePropertyNames, md5PropertyNames);

                    foreach (var collectFileInfo in collectFileInfos)
                    {
                        if (HttpUtils.IsUrl(collectFileInfo.Name))
                        {
                            yield return (null, collectFileInfo.Name);
                        }
                        else
                        {
                            Contract.Assert(!string.IsNullOrEmpty(collectUrl));

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

        public async Task<IEnumerable<(IFileInfo CollectFileInfo, string CollectUrl)>> GetRedirectedCollectItemsAsync(IEnumerable<(IFileInfo CollectFileInfo, string CollectUrl)> collectItems, string dataCollectionName, IEnumerable<KeyValuePair<string, string>> collectHeaders, int maxDegreeOfParallelism, IIdentityServiceClientInfo identityServiceClientInfo, CancellationToken cancellationToken)
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

        private async Task<IEnumerable<(IFileInfo CollectFileInfo, string CollectUrl)>> RedirectCollectItemsAsync(IEnumerable<(IFileInfo CollectFileInfo, string CollectUrl)> collectItems, string dataCollectionName, IEnumerable<KeyValuePair<string, string>> collectHeaders, int maxDegreeOfParallelism, string identityServiceAccessToken, CancellationToken cancellationToken)
        {
            var result = new ConcurrentBag<(IFileInfo CollectFileInfo, string CollectUrl)>();

            await ParallelUtils.ParallelEnumerateAsync(collectItems, cancellationToken, Math.Max(1, maxDegreeOfParallelism),
                async (collectItem, ct) =>
                {
                    var url = await _collectUrlExtractor.ExtractCollectUrlAsync(dataCollectionName, collectItem.CollectFileInfo.Name, collectItem.CollectUrl.TrimStart('@'), collectHeaders, identityServiceAccessToken, ct);
                    result.Add((collectItem.CollectFileInfo, url));
                }
            );

            return result;
        }

        private IEnumerable<string> GetStrings(string json)
        {
            try
            {
                return JsonConvert.DeserializeObject<IEnumerable<string>>(json);
            }
            catch
            {
                return null;
            }
        }

        private IEnumerable<IFileInfo> GetCollectFileInfos(string collectFileInfosJson, IEnumerable<string> namePropertyNames, IEnumerable<string> sizePropertyNames, IEnumerable<string> md5PropertyNames)
        {
            var names = GetStrings(collectFileInfosJson);

            if (names != null)
            {
                foreach (var name in names)
                {
                    yield return _fileInfoFactory(name, null, null, null);
                }
            }
            else
            {
                var files = JArray.Parse(collectFileInfosJson) as JArray;
                var hasResult = false;
                foreach (var file in files)
                {
                    var name = namePropertyNames.Select(x => file.Value<string>(x)).Where(x => !string.IsNullOrEmpty(x)).FirstOrDefault();
                    var sizestr = sizePropertyNames.Select(x => file.Value<string>(x)).Where(x => !string.IsNullOrEmpty(x)).FirstOrDefault();
                    var md5 = md5PropertyNames.Select(x => file.Value<string>(x)).Where(x => !string.IsNullOrEmpty(x)).FirstOrDefault();

                    if (!string.IsNullOrEmpty(name))
                    {
                        var size = long.TryParse(sizestr, out var outsize) ? (long?)outsize : null;
                        hasResult = true;
                        yield return _fileInfoFactory(name, size, md5, null);
                    }
                }

                if (!hasResult)
                {
                    var displayPropertyNames = string.Join("|", namePropertyNames);
                    throw new Exception($"Could not parse collect file identifiers list as JSON array of strings or objects with property '{displayPropertyNames}'");
                }
            }
        }

        private async Task<string> GetCollectItemsJson(string url, IEnumerable<KeyValuePair<string, string>> collectFileIdentifiersHeaders, IIdentityServiceClientInfo identityServiceClientInfo, CancellationToken cancellationToken)
        {
            return
                await HttpUtils.GetStringAsync(url, HttpMethod.Get,
                    headers: collectFileIdentifiersHeaders,
                    accept: "application/json",
                    requestConfiguratorTask: request => _identityServiceHttpRequestConfigurator.ConfigAsync(request, identityServiceClientInfo, cancellationToken),
                    cancellationToken: cancellationToken);
        }
    }
}
