using System.Net.Http.Headers;

namespace Abmes.DataCollector.Utils;

public static class HttpRequestHeadersExtensions
{
    public static void AddValues(this HttpRequestHeaders httpRequestHeaders, IEnumerable<KeyValuePair<string, string>> values)
    {
        foreach (var value in values)
        {
            httpRequestHeaders.Add(value.Key, value.Value);
        }
    }
}
