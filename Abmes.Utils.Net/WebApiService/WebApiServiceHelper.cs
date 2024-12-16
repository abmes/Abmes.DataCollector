using Abmes.Utils.Json;
using System.Reflection;
using System.Text.Json;

namespace Abmes.Utils.Net.WebApiService;

public static class WebApiServiceHelper
{
    private const string AsyncSuffix = "Async";
    private const string RequestParameterName = "request";
    private const string CancellationTokenParameterName = "cancellationToken";

    public static readonly JsonSerializerOptions JsonSerializerOptions = new(JsonSerializerDefaults.Web)
    {
        Converters = { new MemoryStreamJsonConverter() }
    };

    public static string StripAsyncSuffix(string methodName)
    {
        return
            methodName.EndsWith(AsyncSuffix)
            ? methodName[..^AsyncSuffix.Length]
            : methodName;
    }

    public static void CheckTypeIsInterfaceAndAllMethodsHaveGoodSignatureAndName<T>(bool ignoreSynchronousMethodsAndProperties)
        where T : class
    {
        CheckTypeIsInterface<T>();
        CheckAllMethodsHaveGoodSignatureAndName<T>(ignoreSynchronousMethodsAndProperties);
    }

    private static void CheckTypeIsInterface<T>()
        where T : class
    {
        var type = typeof(T);
        ArgumentExceptionExtensions.ThrowIf(!type.IsInterface, $"Type {type.Name} is not an interface");
    }

    private static void CheckAllMethodsHaveGoodSignatureAndName<T>(bool ignoreSynchronousMethodsAndProperties)
        where T : class
    {
        var type = typeof(T);
        var badMethod =
            GetAllMethods<T>(ignoreSynchronousMethodsAndProperties)
            .Where(m => !IsMethodWithGoodSignatureAndName(m))
            .FirstOrDefault();

        ArgumentExceptionExtensions.ThrowIf(badMethod is not null, $"Method {type.Name}.{badMethod?.Name} has a bad signature or name");
    }

    private static bool IsMethodWithGoodSignatureAndName(MethodInfo method)
    {
        return
            IsAsyncMethodWithoutRequestWithoutResponse(method) ||
            IsAsyncMethodWithoutRequestWithResponse(method) ||
            IsAsyncMethodWithRequestWithoutResponse(method) ||
            IsAsyncMethodWithRequestWithResponse(method);
    }

    public static IEnumerable<MethodInfo> GetAllMethods<T>(bool ignoreSynchronousMethodsAndProperties = false)
        where T : class
    {
        var result =
            typeof(T)
            .GetMethods()
            .Where(m => m.DeclaringType != typeof(IDisposable) || m.Name != nameof(IDisposable.Dispose))
            .Where(m => m.DeclaringType != typeof(IAsyncDisposable) || m.Name != nameof(IAsyncDisposable.DisposeAsync));

        return
            ignoreSynchronousMethodsAndProperties
            ? result.Where(m => !IsPropertyGetterOrSetter(m) && IsAsyncMethod(m))
            : result;
    }

    public static bool IsPropertyGetterOrSetter(MethodInfo method)
    {
        return method.Name.StartsWith("get_") || method.Name.StartsWith("set_");
    }

    public static void CheckIsAsyncMethodWithoutRequestWithoutResponse(MethodInfo method)
    {
        ArgumentExceptionExtensions.ThrowIfNot(IsAsyncMethodWithoutRequestWithoutResponse(method));
    }

    private static bool IsAsyncMethodWithoutRequestWithoutResponse(MethodInfo method)
    {
        return
            IsAsyncWithoutResponse(method) &&
            method.GetParameters().Length == 1 &&
            HasCancellationTokenParameter(method);
    }

    public static void CheckIsAsyncMethodWithoutRequestWithResponse(MethodInfo method)
    {
        ArgumentExceptionExtensions.ThrowIfNot(IsAsyncMethodWithoutRequestWithResponse(method));
    }

    private static bool IsAsyncMethodWithoutRequestWithResponse(MethodInfo method)
    {
        return
            IsAsyncWithResponse(method) &&
            method.GetParameters().Length == 1 &&
            HasCancellationTokenParameter(method);
    }

    public static void CheckIsAsyncMethodWithRequestWithoutResponse(MethodInfo method)
    {
        ArgumentExceptionExtensions.ThrowIfNot(IsAsyncMethodWithRequestWithoutResponse(method));
    }

    private static bool IsAsyncMethodWithRequestWithoutResponse(MethodInfo method)
    {
        return
            IsAsyncWithoutResponse(method) &&
            method.GetParameters().Length == 2 &&
            HasRequestParameter(method) &&
            HasCancellationTokenParameter(method);
    }

    public static void CheckIsAsyncMethodWithRequestWithResponse(MethodInfo method)
    {
        ArgumentExceptionExtensions.ThrowIfNot(IsAsyncMethodWithRequestWithResponse(method));
    }

    private static bool IsAsyncMethodWithRequestWithResponse(MethodInfo method)
    {
        return
            IsAsyncWithResponse(method) &&
            method.GetParameters().Length == 2 &&
            HasRequestParameter(method) &&
            HasCancellationTokenParameter(method);
    }

    private static bool HasRequestParameter(MethodInfo method)
    {
        var firstParam = method.GetParameters().FirstOrDefault();

        return
            firstParam is not null &&
            firstParam.Name == RequestParameterName &&
            IsGoodObjectType(firstParam.ParameterType);
    }

    private static bool HasCancellationTokenParameter(MethodInfo method)
    {
        var lastParam = method.GetParameters().LastOrDefault();

        return
            lastParam is not null &&
            lastParam.Name == CancellationTokenParameterName &&
            lastParam.ParameterType == typeof(CancellationToken);
    }

    private static bool IsAsyncWithoutResponse(MethodInfo method)
    {
        return
            HasAsyncName(method) &&
            method.ReturnType == typeof(Task);
    }

    private static bool IsAsyncWithResponse(MethodInfo method)
    {
        return
            HasAsyncName(method) &&
            method.ReturnType.IsGenericType &&
            method.ReturnType.GetGenericTypeDefinition() == typeof(Task<>) &&
            IsGoodObjectType(method.ReturnType.GetGenericArguments()[0]);
    }

    private static bool HasAsyncName(MethodInfo method)
    {
        return method.Name.EndsWith(AsyncSuffix);
    }

    private static bool IsGoodObjectType(Type type)
    {
        return
            type.IsSubclassOf(typeof(object)) &&
            type != typeof(string);
    }

    public static bool IsAsyncMethod(MethodInfo method)
    {
        return
            method.ReturnType == typeof(Task) ||
            method.ReturnType.IsGenericType &&
            method.ReturnType.GetGenericTypeDefinition() == typeof(Task<>);
    }
}
