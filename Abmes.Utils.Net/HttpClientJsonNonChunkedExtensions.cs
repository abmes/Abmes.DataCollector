using System.Diagnostics.CodeAnalysis;
using System.Net.Http.Json;
using System.Text.Json.Serialization.Metadata;
using System.Text.Json;

namespace Abmes.Utils.Net;

public static class HttpClientJsonNonChunkedExtensions
{
    public static async Task<HttpResponseMessage> PostAsJsonNonChunkedAsync<TValue>(
        this HttpClient client,
        [StringSyntax(StringSyntaxAttribute.Uri)] string? requestUri,
        TValue value, JsonSerializerOptions? options,
        CancellationToken cancellationToken)
    {
        JsonContent content = JsonContent.Create(value, mediaType: null, options);
        await content.LoadIntoBufferAsync(); // avoid chunked encoding

        return await client.PostAsync(requestUri, content, cancellationToken);
    }

    public static async Task<HttpResponseMessage> PostAsJsonNonChunkedAsync<TValue>(
        this HttpClient client,
        Uri? requestUri,
        TValue value,
        JsonSerializerOptions? options,
        CancellationToken cancellationToken)
    {
        JsonContent content = JsonContent.Create(value, mediaType: null, options);
        await content.LoadIntoBufferAsync(); // avoid chunked encoding

        return await client.PostAsync(requestUri, content, cancellationToken);
    }

    public static async Task<HttpResponseMessage> PostAsJsonNonChunkedAsync<TValue>(
        this HttpClient client,
        [StringSyntax(StringSyntaxAttribute.Uri)] string? requestUri,
        TValue value,
        CancellationToken cancellationToken)
    {
        return await client.PostAsJsonNonChunkedAsync(requestUri, value, options: null, cancellationToken);
    }

    public static async Task<HttpResponseMessage> PostAsJsonNonChunkedAsync<TValue>(
        this HttpClient client,
        Uri? requestUri,
        TValue value,
        CancellationToken cancellationToken)
    {
        return await client.PostAsJsonNonChunkedAsync(requestUri, value, options: null, cancellationToken);
    }

    public static async Task<HttpResponseMessage> PostAsJsonNonChunkedAsync<TValue>(
        this HttpClient client,
        [StringSyntax(StringSyntaxAttribute.Uri)] string? requestUri,
        TValue value,
        JsonTypeInfo<TValue> jsonTypeInfo,
        CancellationToken cancellationToken)
    {
        JsonContent content = JsonContent.Create(value, jsonTypeInfo);
        await content.LoadIntoBufferAsync(); // avoid chunked encoding

        return await client.PostAsync(requestUri, content, cancellationToken);
    }

    public static async Task<HttpResponseMessage> PostAsJsonNonChunkedAsync<TValue>(
        this HttpClient client,
        Uri? requestUri,
        TValue value,
        JsonTypeInfo<TValue> jsonTypeInfo,
        CancellationToken cancellationToken)
    {
        JsonContent content = JsonContent.Create(value, jsonTypeInfo);
        await content.LoadIntoBufferAsync(); // avoid chunked encoding

        return await client.PostAsync(requestUri, content, cancellationToken);
    }
}
