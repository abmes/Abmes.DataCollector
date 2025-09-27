using System.Net.Http.Json;
using System.Reflection;

namespace Alvecta.Utils.Net.WebApiService;

public class WebApiServiceClientDispatchProxy<TServiceInterface> : DispatchProxy
    where TServiceInterface : class  // TServiceInterface must be an interface
{
    private IHttpClientFactory HttpClientFactory => Ensure.NotNull(_httpClientFactory);
    private IHttpClientFactory? _httpClientFactory;

    private string HttpClientName => Ensure.NotNull(_httpClientName);
    private string? _httpClientName;

    private string RelativeUrl => Ensure.NotNull(_relativeUrl);
    private string? _relativeUrl;

    private string? SingleMethodNameToMap => _singleMethodNameToMap;
    private string? _singleMethodNameToMap;

    private bool IgnoreSynchronousMethodsAndProperties => Ensure.NotNull(_ignoreSynchronousMethodsAndProperties);
    private bool? _ignoreSynchronousMethodsAndProperties;

    private void SetDependencies(
        IHttpClientFactory httpClientFactory,
        string httpClientName,
        string relativeUrl,
        string? singleMethodNameToMap,
        bool ignoreSynchronousMethodsAndProperties)
    {
        ArgumentExceptionExtensions.ThrowIf(_httpClientFactory is not null);
        ArgumentExceptionExtensions.ThrowIf(_httpClientName is not null);
        ArgumentExceptionExtensions.ThrowIf(_relativeUrl is not null);
        ArgumentExceptionExtensions.ThrowIf(_ignoreSynchronousMethodsAndProperties is not null);

        _httpClientFactory = httpClientFactory;
        _httpClientName = httpClientName;
        _relativeUrl = relativeUrl;
        _singleMethodNameToMap = singleMethodNameToMap;
        _ignoreSynchronousMethodsAndProperties = ignoreSynchronousMethodsAndProperties;
    }

    public static TServiceInterface Create(
        IHttpClientFactory httpClientFactory,
        string httpClientName,
        string relativeUrl,
        string? singleMethodNameToMap,
        bool ignoreSynchronousMethodsAndProperties)
    {
        WebApiServiceHelper.CheckTypeIsInterfaceAndAllMethodsHaveGoodSignatureAndName<TServiceInterface>(ignoreSynchronousMethodsAndProperties);

        var proxy = Create<TServiceInterface, WebApiServiceClientDispatchProxy<TServiceInterface>>() as WebApiServiceClientDispatchProxy<TServiceInterface>;
        ArgumentNullException.ThrowIfNull(proxy);
        proxy.SetDependencies(httpClientFactory, httpClientName, relativeUrl, singleMethodNameToMap, ignoreSynchronousMethodsAndProperties);
        return Ensure.NotNull(proxy as TServiceInterface);
    }

    protected override object? Invoke(MethodInfo? targetMethod, object?[]? args)
    {
        ArgumentNullException.ThrowIfNull(targetMethod);
        ArgumentExceptionExtensions.ThrowIf(WebApiServiceHelper.IsPropertyGetterOrSetter(targetMethod));
        ArgumentExceptionExtensions.ThrowIf(SingleMethodNameToMap is not null && targetMethod.Name != SingleMethodNameToMap);

        if (targetMethod.DeclaringType == typeof(IDisposable) && targetMethod.Name == nameof(IDisposable.Dispose))
        {
            return null;
        }

        if (targetMethod.DeclaringType == typeof(IAsyncDisposable) && targetMethod.Name == nameof(IAsyncDisposable.DisposeAsync))
        {
            return ValueTask.CompletedTask;
        }

        ArgumentNullException.ThrowIfNull(args);
        ArgumentExceptionExtensions.ThrowIfNot(args.Length == 1 || args.Length == 2);
        ArgumentExceptionExtensions.ThrowIfNot(args.Length == targetMethod.GetParameters().Length);

        var isAsyncMethod = WebApiServiceHelper.IsAsyncMethod(targetMethod);

        ArgumentExceptionExtensions.ThrowIfNot(isAsyncMethod);

        var requestType =
            args.Length > 1
            ? targetMethod.GetParameters()[0].ParameterType  // first param is request, second is cancellationToken
            : null;

        var returnType =
            isAsyncMethod
            ? targetMethod.ReturnType.IsGenericType
                ? targetMethod.ReturnType.GetGenericArguments()[0]
                : null
            : targetMethod.ReturnType != typeof(void)
                ? targetMethod.ReturnType
                : null;

        var invokerMethod =
            requestType is not null
            ? returnType is not null
                ? GetType()
                    .GetMethod(nameof(DoInvokeWithRequestWithResponseAsync))
                    ?.MakeGenericMethod(requestType, returnType)
                : GetType()
                    .GetMethod(nameof(DoInvokeWithRequestWithoutResponseAsync))
                    ?.MakeGenericMethod(requestType)
            : returnType is not null
                ? GetType()
                    .GetMethod(nameof(DoInvokeWithoutRequestWithResponseAsync))
                    ?.MakeGenericMethod(returnType)
                : GetType()
                    .GetMethod(nameof(DoInvokeWithoutRequestWithoutResponseAsync));

        ArgumentNullException.ThrowIfNull(invokerMethod);

        return invokerMethod.Invoke(this, [targetMethod.Name, .. args]);
    }

    public async Task DoInvokeWithoutRequestWithoutResponseAsync(string methodName, CancellationToken cancellationToken)
    {
        using var httpClient = HttpClientFactory.CreateClient(HttpClientName);

        using var response =
            await httpClient.PostAsync(
                GetMethodUrl(methodName),
                null,
                cancellationToken);

        await response.CheckSuccessAsync(cancellationToken);
    }

    public async Task<TResponse> DoInvokeWithoutRequestWithResponseAsync<TResponse>(string methodName, CancellationToken cancellationToken)
        where TResponse : class
    {
        using var httpClient = HttpClientFactory.CreateClient(HttpClientName);

        using var response =
            await httpClient.PostAsync(
                GetMethodUrl(methodName),
                null,
                cancellationToken);

        await response.CheckSuccessAsync(cancellationToken);

        var result = await response.Content.ReadFromJsonAsync<TResponse>(WebApiServiceHelper.JsonSerializerOptions, cancellationToken);
        return Ensure.NotNull(result);
        // todo: recursively check non-nullable properties of result for null - in .net 9 use JsonSerializerOptions.RespectNullableAnnotations
    }

    public async Task DoInvokeWithRequestWithoutResponseAsync<TRequest>(string methodName, TRequest request, CancellationToken cancellationToken)
        where TRequest : class
    {
        ArgumentNullException.ThrowIfNull(request);  // method is dynamically invoked, so we need to check request here

        using var httpClient = HttpClientFactory.CreateClient(HttpClientName);

        using var response =
            await httpClient.PostAsJsonNonChunkedAsync(
                GetMethodUrl(methodName),
                request,
                WebApiServiceHelper.JsonSerializerOptions,
                cancellationToken);

        await response.CheckSuccessAsync(cancellationToken);
    }

    public async Task<TResponse> DoInvokeWithRequestWithResponseAsync<TRequest, TResponse>(string methodName, TRequest request, CancellationToken cancellationToken)
        where TRequest : class
        where TResponse : class
    {
        ArgumentNullException.ThrowIfNull(request);  // method is dynamically invoked, so we need to check request here

        using var httpClient = HttpClientFactory.CreateClient(HttpClientName);

        using var response =
            await httpClient.PostAsJsonNonChunkedAsync(
                GetMethodUrl(methodName),
                request,
                WebApiServiceHelper.JsonSerializerOptions,
                cancellationToken);

        await response.CheckSuccessAsync(cancellationToken);

        var result = await response.Content.ReadFromJsonAsync<TResponse>(WebApiServiceHelper.JsonSerializerOptions, cancellationToken);
        return Ensure.NotNull(result);
        // todo: recursively check non-nullable properties of result for null - in .net 9 use JsonSerializerOptions.RespectNullableAnnotations
    }

    private string GetMethodUrl(string methodName)
    {
        return
            methodName == SingleMethodNameToMap
            ? RelativeUrl
            : $"{RelativeUrl}/{WebApiServiceHelper.StripAsyncSuffix(methodName)}";
    }
}
