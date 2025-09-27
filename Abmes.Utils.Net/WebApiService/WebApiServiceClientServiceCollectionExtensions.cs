using Alvecta.Utils.Net.WebApiService;

namespace Microsoft.Extensions.DependencyInjection;

public static class WebApiServiceClientServiceCollectionExtensions
{
    public static IServiceCollection AddWebApiServiceClient<TServiceInterface>(
        this IServiceCollection services,
        string? httpClientName = null,
        string? relativeUrl = null,
        bool ignoreSynchronousMethodsAndProperties = false)
        where TServiceInterface : class
    {
        WebApiServiceHelper.CheckTypeIsInterfaceAndAllMethodsHaveGoodSignatureAndName<TServiceInterface>(ignoreSynchronousMethodsAndProperties);

        return
            services.AddTransient<TServiceInterface>(
                provider =>
                    WebApiServiceClientDispatchProxy<TServiceInterface>.Create(
                        provider.GetRequiredService<IHttpClientFactory>(),
                        httpClientName ?? typeof(TServiceInterface).Name,
                        relativeUrl ?? typeof(TServiceInterface).Name,
                        null,
                        ignoreSynchronousMethodsAndProperties));
    }
}
