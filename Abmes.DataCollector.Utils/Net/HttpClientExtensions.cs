using System.Net.Http.Headers;
using System.Text;

namespace Abmes.DataCollector.Utils.Net;

public static class HttpClientExtensions
{
    public static async Task<string> GetStringAsync(
        this HttpClient httpClient,
        string url,
        IEnumerable<KeyValuePair<string, string>>? headers = null,
        string? accept = null,
        TimeSpan? timeout = null,
        CancellationToken cancellationToken = default)
    {
        return await httpClient.GetStringAsync(url, HttpMethod.Get, null, null, headers, accept, timeout, null, cancellationToken);
    }

    public static async Task<string> GetStringAsync(
        this HttpClient httpClient,
        string url,
        HttpMethod httpMethod,
        string? body = null,
        string? bodyMediaType = null,
        IEnumerable<KeyValuePair<string, string>>? headers = null,
        string? accept = null,
        TimeSpan? timeout = null,
        Func<HttpRequestMessage, CancellationToken, Task>? requestConfiguratorTask = null,
        CancellationToken cancellationToken = default)
    {
        using var response =
            await httpClient.SendAsync(
                url,
                httpMethod,
                body,
                bodyMediaType,
                null,
                headers,
                accept,
                timeout,
                requestConfiguratorTask,
                HttpCompletionOption.ResponseContentRead,
                cancellationToken);

        return await response.Content.ReadAsStringAsync(cancellationToken);
    }

    public static async Task<HttpResponseMessage> SendAsync(
        this HttpClient httpClient,
        string url,
        HttpMethod httpMethod,
        string? body = null,
        string? bodyMediaType = null,
        Stream? content = null,
        IEnumerable<KeyValuePair<string, string>>? headers = null,
        string? accept = null,
        TimeSpan? timeout = null,
        Func<HttpRequestMessage, CancellationToken, Task>? requestConfiguratorTask = null,
        HttpCompletionOption httpCompletionOption = default,
        CancellationToken cancellationToken = default)
    {
        if (timeout is not null)
        {
            httpClient.Timeout = timeout.Value;
        }

        using var request = new HttpRequestMessage(httpMethod, url);

        if (headers is not null)
        {
            request.Headers.AddValues(headers);
        }

        if (!string.IsNullOrEmpty(accept))
        {
            request.Headers.Accept.Add(new MediaTypeWithQualityHeaderValue(accept));
        }

        if (!string.IsNullOrEmpty(body))
        {
            request.Content = string.IsNullOrEmpty(bodyMediaType) ? new StringContent(body) : new StringContent(body, Encoding.UTF8, bodyMediaType);
        }

        if (content is not null)
        {
            request.Content = new StreamContent(content);
        }

        if (requestConfiguratorTask is not null)
        {
            await requestConfiguratorTask(request, cancellationToken);
        }

        var response = await httpClient.SendAsync(request, httpCompletionOption, cancellationToken);

        await response.CheckSuccessAsync(cancellationToken);

        return response;
    }

    public static async Task SendManyAsync(
        this HttpClient httpClient,
        IEnumerable<string> urls,
        HttpMethod httpMethod,
        IEnumerable<KeyValuePair<string, string>>? headers = null,
        CancellationToken cancellationToken = default)
    {
        foreach (var u in urls.SkipLast(1))
        {
            using var _ = await httpClient.SendAsync(u, httpMethod, headers: headers, cancellationToken: cancellationToken);
        }
    }

    public static async Task<string> ProxiedGetStringAsync(
        this HttpClient httpClient,
        string httpProxyUrl,
        string httpProxyForwardUrlHeaderName,
        string url,
        IEnumerable<KeyValuePair<string, string>>? headers = null,
        string? accept = null,
        TimeSpan? timeout = null,
        CancellationToken cancellationToken = default)
    {
        headers ??= [];
        headers = headers.Append(new (httpProxyForwardUrlHeaderName, url));

        return await httpClient.GetStringAsync(httpProxyUrl, headers, accept, timeout, cancellationToken);
    }
}
