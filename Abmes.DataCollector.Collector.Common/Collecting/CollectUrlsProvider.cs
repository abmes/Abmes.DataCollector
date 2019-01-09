﻿using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Abmes.DataCollector.Collector.Common.Misc;
using System;
using System.Collections.Generic;
using System.Diagnostics.Contracts;
using System.Linq;
using System.Text;
using Abmes.DataCollector.Utils;

namespace Abmes.DataCollector.Collector.Common.Collecting
{
    public class CollectUrlsProvider : ICollectUrlsProvider
    {
        private static readonly string[] DefaultIdentifierPropertyNames = { "name", "fileName", "identifier" };

        private IEnumerable<string> GenerateCollectUrls(string collectFileIdentifiersUrl, IEnumerable<KeyValuePair<string, string>> collectFileIdentifiersHeaders, string collectUrl, IEnumerable<KeyValuePair<string, string>> collectHeaders)
        {
            if (string.IsNullOrEmpty(collectFileIdentifiersUrl))
            {
                yield return collectUrl;
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
                            yield return collectFileIdentifier;
                        }
                        else
                        {
                            Contract.Assert(!string.IsNullOrEmpty(collectUrl));

                            if ((string.IsNullOrEmpty(queryFilter)) ||
                                (FileMaskUtils.FileNameMatchesFilter(collectFileIdentifier, queryFilter)))
                            {
                                yield return collectUrl.Replace("[filename]", collectFileIdentifier, StringComparison.InvariantCultureIgnoreCase);
                            }
                        }
                    }
                }
            }
        }

        private IEnumerable<string> GetCollectUrlsFrom(string sourceUrl, IEnumerable<KeyValuePair<string, string>> headers)
        {
            var collectUrlsJson = HttpUtils.GetString(sourceUrl, headers, "application/json").Result;
            foreach (var url in GetCollectUrlsFromJson(collectUrlsJson))
            {
                yield return url;
            }
        }

        public IEnumerable<string> GetCollectUrls(string collectFileIdentifiersUrl, IEnumerable<KeyValuePair<string, string>> collectFileIdentifiersHeaders, string collectUrl, IEnumerable<KeyValuePair<string, string>> collectHeaders)
        {
            var collectUrls = GenerateCollectUrls(collectFileIdentifiersUrl, collectFileIdentifiersHeaders, collectUrl, collectHeaders).ToList();

            return
                collectUrls
                .Where(x => !x.StartsWith('@'))
                .Concat(
                    collectUrls
                    .Where(x => x.StartsWith('@'))
                    .SelectMany(x => GetCollectUrlsFrom(x.TrimStart('@'), collectHeaders))
                );
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

        private string TrimPseudoNewLine(string s)
        {
            while ((!string.IsNullOrEmpty(s)) && (s.EndsWith(@"\n") || s.EndsWith(@"\r")))
            {
                s = s.Remove(s.Length - 2);
            }

            return s;
        }

        private IEnumerable<string> GetCollectUrlsFromJson(string json)
        {
            try
            {
                return JsonConvert.DeserializeObject<IEnumerable<string>>(json).Select(x => TrimPseudoNewLine(x?.Trim()));
            }
            catch
            {
                return new[] { TrimPseudoNewLine(json.Trim('"').Trim()) };
            }
        }
    }
}
