namespace Abmes.DataCollector.Utils;

public interface IAsyncExecutionStrategy<T> : IAsyncExecutionStrategy
{
    // nothing here. just a marker interface
}

public interface IAsyncExecutionStrategy
{
    ValueTask ExecuteAsync(Func<CancellationToken, ValueTask> func, CancellationToken cancellationToken);
    ValueTask<TResult> ExecuteAsync<TResult>(Func<CancellationToken, ValueTask<TResult>> func, CancellationToken cancellationToken);
}
