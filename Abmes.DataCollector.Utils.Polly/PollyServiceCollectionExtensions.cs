using Microsoft.Extensions.DependencyInjection;
using Polly;
using Polly.DependencyInjection;

namespace Abmes.DataCollector.Utils.Polly;

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
