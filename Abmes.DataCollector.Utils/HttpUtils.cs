using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading;
using System.Threading.Tasks;

namespace Abmes.DataCollector.Utils
{
    public static class HttpUtils
    {
        public static async Task<string> GetString(string url, IEnumerable<KeyValuePair<string, string>> headers = null, string accept = null, CancellationToken cancellationToken = default(CancellationToken))
        {
            if (cancellationToken == default(CancellationToken))
            {
                cancellationToken = CancellationToken.None;
            }

            using (var httpClient = new HttpClient())
            {
                using (var request = new HttpRequestMessage(HttpMethod.Get, url))
                {
                    if (headers != null)
                    {
                        request.Headers.AddValues(headers);
                    }

                    if (!string.IsNullOrEmpty(accept))
                    {
                        request.Headers.Accept.Add(new MediaTypeWithQualityHeaderValue(accept));
                    }

                    var response = await httpClient.SendAsync(request, cancellationToken);

                    await response.CheckSuccessAsync();

                    return await response.Content.ReadAsStringAsync();
                }
            }
        }

        public static bool IsUrl(string url)
        {
            return
                !string.IsNullOrEmpty(url) &&
                (url.StartsWith("http://") || url.StartsWith("https://"));
        }

        public static async Task CheckSuccessAsync(this HttpResponseMessage response)
        {
            if (!response.IsSuccessStatusCode)
            {
                var errorMessage = await response.Content.ReadAsStringAsync();
                //errorMessage = errorMessage.Substring(0, Math.Min(errorMessage.Length, 1000));
                throw new HttpRequestException(response.ReasonPhrase + " " + (int)response.StatusCode + Environment.NewLine + errorMessage);
            }
        }

        public static void AddValues(this HttpRequestHeaders httpRequestHeaders, IEnumerable<KeyValuePair<string, string>> values)
        {
            if (values != null)
            {
                foreach (var value in values)
                {
                    httpRequestHeaders.Add(value.Key, value.Value);
                }
            }
        }
    }
}
