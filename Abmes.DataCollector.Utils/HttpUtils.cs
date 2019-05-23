using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading;
using System.Threading.Tasks;

namespace Abmes.DataCollector.Utils
{
    public static class HttpUtils
    {
        public static async Task<string> GetStringAsync(string url, IEnumerable<KeyValuePair<string, string>> headers = null, string accept = null, TimeSpan? timeout = null, CancellationToken cancellationToken = default(CancellationToken))
        {
            return await GetStringAsync(url, HttpMethod.Get, null, headers, accept, timeout, null, cancellationToken);
        }

        public static async Task<string> GetStringAsync(string url, HttpMethod httpMethod, string body = null, IEnumerable<KeyValuePair<string, string>> headers = null, string accept = null, TimeSpan? timeout = null, Func<HttpRequestMessage, Task> requestConfiguratorTask = null, CancellationToken cancellationToken = default(CancellationToken))
        {
            using (var response = await SendAsync(url, httpMethod, body, null, headers, accept, timeout, requestConfiguratorTask, HttpCompletionOption.ResponseContentRead, cancellationToken))
            {
                return await response.Content.ReadAsStringAsync();
            }
        }

        public static async Task<HttpResponseMessage> SendAsync(string url, HttpMethod httpMethod, string body = null, Stream content = null, IEnumerable<KeyValuePair<string, string>> headers = null, string accept = null, TimeSpan? timeout = null, Func<HttpRequestMessage, Task> requestConfiguratorTask = null, HttpCompletionOption httpCompletionOption = default(HttpCompletionOption), CancellationToken cancellationToken = default(CancellationToken))
        {
            if (cancellationToken == default(CancellationToken))
            {
                cancellationToken = CancellationToken.None;
            }

            using (var httpClient = new HttpClient())
            {
                if (timeout.HasValue)
                {
                    httpClient.Timeout = timeout.Value;
                }

                using (var request = new HttpRequestMessage(httpMethod, url))
                {
                    if (headers != null)
                    {
                        request.Headers.AddValues(headers);
                    }

                    if (!string.IsNullOrEmpty(accept))
                    {
                        request.Headers.Accept.Add(new MediaTypeWithQualityHeaderValue(accept));
                    }

                    if (!string.IsNullOrEmpty(body))
                    {
                        request.Content = new StringContent(body);
                    }

                    if (content != null)
                    {
                        request.Content = new StreamContent(content);
                    }

                    if (requestConfiguratorTask != null)
                    {
                        await requestConfiguratorTask(request);
                    }

                    var response = await httpClient.SendAsync(request, httpCompletionOption, cancellationToken);

                    await response.CheckSuccessAsync();

                    return response;
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

        public static string ContentMD5(this HttpResponseMessage response)
        {
            return
                CopyUtils.GetMD5Hash(response.Content.Headers.ContentMD5) ??
                response.Headers.Where(x => x.Key.Equals("x-amz-meta-content-md5", StringComparison.InvariantCultureIgnoreCase)).Select(z => z.Value.FirstOrDefault()).FirstOrDefault();
        }

        public static IEnumerable<KeyValuePair<string, string>> GetHeaders(string headers)
        {
            return
                headers.Split(';', ',')
                .Select(x => { var parts = x.Split('='); return new KeyValuePair<string, string>(parts[0].Trim(), parts[1].Trim()); });
        }

        public static string FixJsonResult(string json)
        {
            var s = json.Trim('"').Trim();

            while ((!string.IsNullOrEmpty(s)) && (s.EndsWith(@"\n") || s.EndsWith(@"\r")))
            {
                s = s.Remove(s.Length - 2);
            }

            return s;
        }
    }
}
