using System.Diagnostics.CodeAnalysis;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using System.Text;

namespace Abmes.DataCollector.Utils.Net;

public static class HttpClientExtensions
{
    public static async Task<HttpResponseMessage> CheckedSendAsync(
        this HttpClient httpClient,
        HttpRequestMessage request,
        HttpCompletionOption httpCompletionOption = HttpCompletionOption.ResponseContentRead,
        CancellationToken cancellationToken = default)
    {
        var response = await httpClient.SendAsync(request, httpCompletionOption, cancellationToken);

        await response.CheckSuccessAsync(cancellationToken);

        return response;
    }

    public static async Task<HttpResponseMessage> CheckedGetAsync(
        this HttpClient httpClient,
        string url,
        HttpCompletionOption httpCompletionOption = HttpCompletionOption.ResponseContentRead,
        IEnumerable<KeyValuePair<string, string>>? headers = null,
        CancellationToken cancellationToken = default)
    {
        using var request = new HttpRequestMessage(HttpMethod.Get, url);

        if (headers is not null)
        {
            request.Headers.AddValues(headers);
        }

        return await httpClient.CheckedSendAsync(request, httpCompletionOption, cancellationToken);
    }

    public static async Task<HttpResponseMessage> GetAsync(
        this HttpClient httpClient,
        string url,
        HttpCompletionOption httpCompletionOption = HttpCompletionOption.ResponseContentRead,
        IEnumerable<KeyValuePair<string, string>>? headers = null,
        CancellationToken cancellationToken = default)
    {
        using var request = new HttpRequestMessage(HttpMethod.Get, url);

        if (headers is not null)
        {
            request.Headers.AddValues(headers);
        }

        return await httpClient.SendAsync(request, httpCompletionOption, cancellationToken);
    }

    public static async Task<string> CheckedGetStringAsync(
        this HttpClient httpClient,
        string url,
        IEnumerable<KeyValuePair<string, string>>? headers = null,
        CancellationToken cancellationToken = default)
    {
        var response = await httpClient.CheckedGetAsync(url, headers: headers, cancellationToken: cancellationToken);
        return await response.Content.ReadAsStringAsync(cancellationToken);
    }

    public static async Task<Stream> CheckedGetStreamAsync(
        this HttpClient httpClient,
        string url,
        IEnumerable<KeyValuePair<string, string>>? headers = null,
        CancellationToken cancellationToken = default)
    {
        var response = await httpClient.CheckedGetAsync(url, HttpCompletionOption.ResponseHeadersRead, headers, cancellationToken);
        return await response.Content.ReadAsStreamAsync(cancellationToken);
    }

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
            request.Content =
                string.IsNullOrEmpty(bodyMediaType)
                ? new StringContent(body)
                : new StringContent(body, Encoding.UTF8, bodyMediaType);
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
            // todo: what if response is not OK?
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

    public static async Task<TValue?> GetNullableFromJsonAsync<TValue>(
        this HttpClient client,
        [StringSyntax("Uri")] string? requestUri,
        CancellationToken cancellationToken)
        where TValue : class?
    {
        var response = await client.GetAsync(requestUri, cancellationToken);

        await response.CheckSuccessAsync(cancellationToken);

        return
            (response.Content.Headers.ContentLength == 0)
            ? null
            : await response.Content.ReadFromJsonAsync<TValue>(cancellationToken);
    }

    public static async Task<TResult?> GetNullableFromPostAsJsonAsync<TResult, TValue>(
        this HttpClient client,
        [StringSyntax("Uri")] string? requestUri,
        TValue value,
        CancellationToken cancellationToken)
        where TResult : class?
    {
        var response = await client.PostAsJsonAsync(requestUri, value, cancellationToken);
        response.EnsureSuccessStatusCode();

        return
            (response.Content.Headers.ContentLength == 0)
            ? null
            : await response.Content.ReadFromJsonAsync<TResult>(cancellationToken);
    }

    public static async Task<IEnumerable<TResultItem>> GetEnumerableFromPostAsJsonAsync<TResultItem, TValue>(
        this HttpClient client,
        [StringSyntax("Uri")] string? requestUri,
        TValue value,
        CancellationToken cancellationToken)
    {
        var response = await client.PostAsJsonAsync(requestUri, value, cancellationToken);

        await response.CheckSuccessAsync(cancellationToken);

        return
            (response.Content.Headers.ContentLength == 0)
            ? []
            : await response.Content.ReadFromJsonAsync<IEnumerable<TResultItem>>(cancellationToken) ?? [];
    }
}
