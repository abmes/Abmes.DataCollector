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

namespace Abmes.DataCollector.Collector.Common.Collecting
{
    public class CollectUrlsProvider : ICollectUrlsProvider
    {
        private readonly ICollectUrlExtractor _collectUrlExtractor;

        public CollectUrlsProvider(
            ICollectUrlExtractor collectUrlsExtractor)
        {
            _collectUrlExtractor = collectUrlsExtractor;
        }

        private static readonly string[] DefaultIdentifierPropertyNames = { "name", "fileName", "identifier" };

        private IEnumerable<(string CollectFileIdentifier, string CollectUrl)> GenerateCollectInfos(string collectFileIdentifiersUrl, IEnumerable<KeyValuePair<string, string>> collectFileIdentifiersHeaders, string collectUrl, IEnumerable<KeyValuePair<string, string>> collectHeaders)
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
                var queryIdentifierPropertyName = selector.Skip(1).FirstOrDefault();

                var collectFileIdentifiersJson = HttpUtils.GetString(queryUrl, collectFileIdentifiersHeaders, "application/json").Result;

                if (!string.IsNullOrEmpty(collectFileIdentifiersJson))
                {
                    var identifierPropertyNames = string.IsNullOrEmpty(queryIdentifierPropertyName) ? DefaultIdentifierPropertyNames : new[] { queryIdentifierPropertyName };

                    var collectFileIdentifiers = GetCollectFileIdentifiers(collectFileIdentifiersJson, identifierPropertyNames);

                    foreach (var collectFileIdentifier in collectFileIdentifiers)
                    {
                        if (HttpUtils.IsUrl(collectFileIdentifier))
                        {
                            yield return (null, collectFileIdentifier);
                        }
                        else
                        {
                            Contract.Assert(!string.IsNullOrEmpty(collectUrl));

                            if ((string.IsNullOrEmpty(queryFilter)) ||
                                (FileMaskUtils.FileNameMatchesFilter(collectFileIdentifier, queryFilter)))
                            {
                                yield return (collectFileIdentifier, collectUrl.Replace("[filename]", collectFileIdentifier, StringComparison.InvariantCultureIgnoreCase));
                            }
                        }
                    }
                }
            }
        }

        public IEnumerable<string> GetCollectUrls(string dataCollectionName, string collectFileIdentifiersUrl, IEnumerable<KeyValuePair<string, string>> collectFileIdentifiersHeaders, string collectUrl, IEnumerable<KeyValuePair<string, string>> collectHeaders, int maxDegreeOfParallelism, CancellationToken cancellationToken)
        {
            var collectInfos = GenerateCollectInfos(collectFileIdentifiersUrl, collectFileIdentifiersHeaders, collectUrl, collectHeaders).ToList();

            return
                collectInfos
                .Where(x => !x.CollectUrl.StartsWith('@'))
                .Select(x => x.CollectUrl)
                .Concat(ExtractUrls(collectInfos.Where(x => x.CollectUrl.StartsWith('@')), dataCollectionName, collectHeaders, maxDegreeOfParallelism, cancellationToken))
                .ToList();
        }

        private IEnumerable<string> ExtractUrls(IEnumerable<(string CollectFileIdentifier, string CollectUrl)> extractInfos, string dataCollectionName, IEnumerable<KeyValuePair<string, string>> collectHeaders, int maxDegreeOfParallelism, CancellationToken cancellationToken)
        {
            var dataflowBlockOptions = new System.Threading.Tasks.Dataflow.ExecutionDataflowBlockOptions
            {
                MaxDegreeOfParallelism = Math.Max(1, maxDegreeOfParallelism),
                CancellationToken = cancellationToken
            };

            var result = new ConcurrentBag<string>();

            var workerBlock = new System.Threading.Tasks.Dataflow.ActionBlock<(string CollectFileIdentifier, string CollectUrl)>(
                extractInfo => result.Add(_collectUrlExtractor.ExtractCollectUrl(dataCollectionName, extractInfo.CollectFileIdentifier, extractInfo.CollectUrl.TrimStart('@'), collectHeaders, cancellationToken)),
                dataflowBlockOptions);

            foreach (var extractInfo in extractInfos)
            {
                workerBlock.Post(extractInfo);
            }

            workerBlock.Complete();

            workerBlock.Completion.Wait();

            return result;
        }

        private IEnumerable<string> GetCollectFileIdentifiers(string collectFileIdentifiersJson, IEnumerable<string> identifierPropertyNames)
        {
            try
            {
                return JsonConvert.DeserializeObject<IEnumerable<string>>(collectFileIdentifiersJson);
            }
            catch
            {
            }

            var files = JArray.Parse(collectFileIdentifiersJson) as JArray;
            foreach (var propertyName in identifierPropertyNames)
            {
                var result = files.Select(x => x.Value<string>(propertyName));
                if (result.Any(x => !string.IsNullOrEmpty(x)))
                {
                    return result;
                }
            }

            var displayPropertyNames = string.Join("|", identifierPropertyNames);
            throw new Exception($"Could not parse collect file identifiers list as JSON array of strings or objects with property '{displayPropertyNames}'");
        }
    }
}
