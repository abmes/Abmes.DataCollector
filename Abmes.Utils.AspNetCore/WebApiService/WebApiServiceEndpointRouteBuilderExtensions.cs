using Abmes.Utils.Net.WebApiService;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Routing;
using System.Diagnostics.CodeAnalysis;
using System.Linq.Expressions;
using System.Reflection;

namespace Abmes.Utils.AspNetCore.WebApiService;

public static class WebApiServiceEndpointRouteBuilderExtensions
{
    public static IEndpointConventionBuilder MapWebApiService<TServiceInterface>(
        this IEndpointRouteBuilder endpoints,
        [StringSyntax("Route")] string? pattern = null,
        bool ignoreSynchronousMethodsAndProperties = false)
        where TServiceInterface : class
    {
        WebApiServiceHelper.CheckTypeIsInterfaceAndAllMethodsHaveGoodSignatureAndName<TServiceInterface>(ignoreSynchronousMethodsAndProperties);

        var result = endpoints.MapGroup(pattern ?? typeof(TServiceInterface).Name);

        var methods = GetAllMappableMethodDelegates<TServiceInterface>();
        foreach (var m in methods)
        {
            result.MapPost($"/{WebApiServiceHelper.StripAsyncSuffix(m.MethodName)}", m.Delegate);
        }

        return result;
    }

    private static IEnumerable<(string MethodName, Delegate Delegate)> GetAllMappableMethodDelegates<TServiceInterface>()
        where TServiceInterface : class
    {
        return
            WebApiServiceHelper.GetAllMethods<TServiceInterface>(true)
            .Select(m => (m.Name, CreateDelegate<TServiceInterface>(m)));
    }

    private static Delegate CreateDelegate<TServiceInterface>(MethodInfo method)
        where TServiceInterface : class
    {
        var methodParameters = method.GetParameters();

        var isAsyncMethod = WebApiServiceHelper.IsAsyncMethod(method);

        ArgumentExceptionExtensions.ThrowIfNot(isAsyncMethod);

        var requestType =
            methodParameters.Length > 1
            ? methodParameters[0].ParameterType
            : null;

        var returnType =
            isAsyncMethod
            ? method.ReturnType.IsGenericType
                ? method.ReturnType.GetGenericArguments()[0]
                : null
            : method.ReturnType != typeof(void)
                ? method.ReturnType
                : null;

        var invokerMethod =
            requestType is not null
            ? returnType is not null
                ? typeof(SimpleMethodInvoker<TServiceInterface>)
                    .GetMethod(nameof(SimpleMethodInvoker<TServiceInterface>.InvokeWithRequestWithResponseAsync))
                    ?.MakeGenericMethod(requestType, returnType)
                : typeof(SimpleMethodInvoker<TServiceInterface>)
                    .GetMethod(nameof(SimpleMethodInvoker<TServiceInterface>.InvokeWithRequestWithoutResponseAsync))
                    ?.MakeGenericMethod(requestType)
            : returnType is not null
                ? typeof(SimpleMethodInvoker<TServiceInterface>)
                    .GetMethod(nameof(SimpleMethodInvoker<TServiceInterface>.InvokeWithoutRequestWithResponseAsync))
                    ?.MakeGenericMethod(returnType)
                : typeof(SimpleMethodInvoker<TServiceInterface>)
                    .GetMethod(nameof(SimpleMethodInvoker<TServiceInterface>.InvokeWithoutRequestWithoutResponseAsync));

        ArgumentNullException.ThrowIfNull(invokerMethod);

        var invokerMethodParameterTypes = invokerMethod.GetParameters().Select(p => p.ParameterType);
        var delegateType = Expression.GetDelegateType([.. invokerMethodParameterTypes, invokerMethod.ReturnType]);

        return invokerMethod.CreateDelegate(delegateType, new SimpleMethodInvoker<TServiceInterface>(method));
    }

    private class SimpleMethodInvoker<TServiceInterface>(
        MethodInfo method)
        where TServiceInterface : class
    {
        public async Task InvokeWithoutRequestWithoutResponseAsync(TServiceInterface instance, CancellationToken cancellationToken)
        {
            WebApiServiceHelper.CheckIsAsyncMethodWithoutRequestWithoutResponse(method);

            await (Task)Ensure.NotNull(method.Invoke(instance, [cancellationToken]));
        }

        public async Task<TResponse> InvokeWithoutRequestWithResponseAsync<TResponse>(TServiceInterface instance, CancellationToken cancellationToken)
            where TResponse : class
        {
            WebApiServiceHelper.CheckIsAsyncMethodWithoutRequestWithResponse(method);

            var result = await (Task<TResponse>)Ensure.NotNull(method.Invoke(instance, [cancellationToken]));

            return Ensure.NotNull(result);
        }

        public async Task InvokeWithRequestWithoutResponseAsync<TRequest>(TServiceInterface instance, TRequest request, CancellationToken cancellationToken)
            where TRequest : class
        {
            WebApiServiceHelper.CheckIsAsyncMethodWithRequestWithoutResponse(method);
            ArgumentNullException.ThrowIfNull(request);  // method is dynamically invoked, so we need to check request here

            await (Task)Ensure.NotNull(method.Invoke(instance, [request, cancellationToken]));
        }

        public async Task<TResponse> InvokeWithRequestWithResponseAsync<TRequest, TResponse>(TServiceInterface instance, TRequest request, CancellationToken cancellationToken)
            where TRequest : class
            where TResponse : class
        {
            WebApiServiceHelper.CheckIsAsyncMethodWithRequestWithResponse(method);
            ArgumentNullException.ThrowIfNull(request);  // method is dynamically invoked, so we need to check request here

            var result = await (Task<TResponse>)Ensure.NotNull(method.Invoke(instance, [request, cancellationToken]));

            return Ensure.NotNull(result);
        }
    }
}
