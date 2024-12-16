using Abmes.Utils.ExecutionStrategy;
using Polly;
using Polly.DependencyInjection;

namespace Microsoft.Extensions.DependencyInjection;

public static class PollyServiceCollectionExtensions
{
    public static IServiceCollection AddPollyAsyncExecutionStrategy<T>(
        this IServiceCollection services,
        Action<ResiliencePipelineBuilder, AddResiliencePipelineContext<Type>> configure) where T : notnull
    {
        return
            services
            .AddTransient<IAsyncExecutionStrategy<T>, PollyAsyncExecutionStrategy<T>>()
            .AddResiliencePipeline(typeof(T), configure);
    }
}
