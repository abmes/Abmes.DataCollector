using Polly;
using Polly.Registry;

namespace Abmes.DataCollector.Utils.Polly;

public class PollyAsyncExecutionStrategy<T>(
    ResiliencePipelineProvider<Type> resiliencePipelineProvider) : IAsyncExecutionStrategy<T>
{
    private readonly ResiliencePipeline _resiliencePipeline = resiliencePipelineProvider.GetPipeline(typeof(T));

    public async ValueTask ExecuteAsync(Func<CancellationToken, ValueTask> func, CancellationToken cancellationToken)
    {
        await _resiliencePipeline.ExecuteAsync(func, cancellationToken);
    }

    public ValueTask<TResult> ExecuteAsync<TResult>(Func<CancellationToken, ValueTask<TResult>> func, CancellationToken cancellationToken)
    {
        return _resiliencePipeline.ExecuteAsync(func, cancellationToken);
    }
}
