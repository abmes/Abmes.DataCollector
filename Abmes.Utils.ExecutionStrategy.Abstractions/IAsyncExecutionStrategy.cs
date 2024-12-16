namespace Abmes.Utils.ExecutionStrategy;

public interface IAsyncExecutionStrategy<T> : IAsyncExecutionStrategy
{
    // nothing here. just a marker interface
}

public interface IAsyncExecutionStrategy
{
    // todo: add default implementation of overloads with 2 and 3 parameters
    ValueTask ExecuteAsync(Func<CancellationToken, ValueTask> func, CancellationToken cancellationToken);
    ValueTask<TResult> ExecuteAsync<TResult>(Func<CancellationToken, ValueTask<TResult>> func, CancellationToken cancellationToken);
}
