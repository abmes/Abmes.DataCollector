using Alvecta.Utils;
using Alvecta.Utils.Net.WebApiService;
using System.Diagnostics.CodeAnalysis;

namespace Microsoft.Extensions.DependencyInjection;

public static class WebApiUseCaseClientServiceCollectionExtensions
{
    public static IServiceCollection AddWebApiUseCaseClient<TUseCaseInterface>(
        this IServiceCollection services,
        string httpClientName,
        string? relativeUrl = null)
        where TUseCaseInterface : class
    {
        ArgumentExceptionExtensions.ThrowIfNot(HasGoodUseCaseInterfaceName<TUseCaseInterface>());

        WebApiServiceHelper.CheckTypeIsInterfaceAndAllMethodsHaveGoodSignatureAndName<TUseCaseInterface>(false);

        return
            services.AddTransient<TUseCaseInterface>(
                provider =>
                    WebApiServiceClientDispatchProxy<TUseCaseInterface>.Create(
                        provider.GetRequiredService<IHttpClientFactory>(),
                        httpClientName,
                        ExpandMacros<TUseCaseInterface>(relativeUrl ?? UseCaseMacro),
                        ExecuteAsyncMethodName,
                        false));
    }

    [return: NotNullIfNotNull(nameof(text))]
    private static string? ExpandMacros<TUseCaseInterface>(string? text)
        where TUseCaseInterface : class
    {
        return text?.Replace(UseCaseMacro, GetUseCaseName<TUseCaseInterface>());
    }

    private static string GetUseCaseName<TUseCaseInterface>()
        where TUseCaseInterface : class
    {
        ArgumentExceptionExtensions.ThrowIfNot(HasGoodUseCaseInterfaceName<TUseCaseInterface>());

        return typeof(TUseCaseInterface).Name[UseCasePrefix.Length..^UseCaseSuffix.Length];
    }


    private static bool HasGoodUseCaseInterfaceName<TUseCaseInterface>()
        where TUseCaseInterface : class
    {
        var useCaseName = typeof(TUseCaseInterface).Name;

        return
            typeof(TUseCaseInterface).IsInterface
            &&
            useCaseName.StartsWith(UseCasePrefix)
            &&
            useCaseName.EndsWith(UseCaseSuffix);
    }

    private const string UseCasePrefix = "I";
    private const string UseCaseSuffix = "UseCase";
    private const string UseCaseMacro = "{UseCase}";
    private const string ExecuteAsyncMethodName = "ExecuteAsync";
}
