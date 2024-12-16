using System.Linq.Expressions;

namespace Abmes.Utils;

public static class LinqExtensions
{
    public static async Task ForEachAsync<TSource>(
        this IEnumerable<TSource> source,
        Func<TSource, ValueTask> body)
    {
        foreach (var item in source)
        {
            await body(item);
        }
    }

    public static async Task ForEachAsync<TSource>(
        this IEnumerable<TSource> source,
        Func<TSource, CancellationToken, ValueTask> body,
        CancellationToken cancellationToken)
    {
        await ForEachAsync(source, async x => await body(x, cancellationToken));
    }

    public static async Task<TResult[]> ForEachAsync<TSource, TResult>(
        this IEnumerable<TSource> source,
        Func<TSource, ValueTask<TResult>> body)
    {
        var results = new List<TResult>();

        if (source.TryGetNonEnumeratedCount(out int count))
        {
            results.Capacity = count;
        }

        foreach (var item in source)
        {
            var result = await body(item);
            results.Add(result);
        }

        return results.ToArray();
    }

    public static async Task<TResult[]> ForEachAsync<TSource, TResult>(
        this IEnumerable<TSource> source,
        Func<TSource, CancellationToken, ValueTask<TResult>> body,
        CancellationToken cancellationToken)
    {
        return await ForEachAsync(source, async x => await body(x, cancellationToken));
    }
    
    public static async Task ParallelForEachAsync<TSource>(
        this IEnumerable<TSource> source,
        Func<TSource, ValueTask> body,
        CancellationToken cancellationToken)
    {
        await Parallel.ForEachAsync(source, cancellationToken, async (x, ct) => await body(x));
    }

    public static async Task ParallelForEachAsync<TSource>(
        this IEnumerable<TSource> source,
        Func<TSource, CancellationToken, ValueTask> body,
        CancellationToken cancellationToken)
    {
        await Parallel.ForEachAsync(source, cancellationToken, body);
    }

    public static async Task ParallelForEachAsync<TSource>(
        this IEnumerable<TSource> source,
        Func<TSource, Task> body,
        int degreeOfParallelism,
        CancellationToken cancellationToken)
    {
        await ParallelForEachAsync(source, async (x, ct) => await body(x), degreeOfParallelism, cancellationToken);
    }

    public static async Task ParallelForEachAsync<TSource>(
        this IEnumerable<TSource> source,
        Func<TSource, CancellationToken, ValueTask> body,
        int degreeOfParallelism,
        CancellationToken cancellationToken)
    {
        var parallelOptions = new ParallelOptions
        {
            MaxDegreeOfParallelism = degreeOfParallelism,
            CancellationToken = cancellationToken
        };
        
        await ParallelForEachAsync(source, parallelOptions, body);
    }

    public static async Task ParallelForEachAsync<TSource>(
        this IEnumerable<TSource> source,
        ParallelOptions parallelOptions,
        Func<TSource, CancellationToken, ValueTask> body)
    {
        await Parallel.ForEachAsync(source, parallelOptions, body);
    }

    public static async Task<TResult[]> ParallelForEachAsync<TSource, TResult>(
        this IEnumerable<TSource> source,
        Func<TSource, ValueTask<TResult>> body,
        CancellationToken cancellationToken)
    {
        return await ParallelForEachAsync(source, body, 0, cancellationToken);
    }

    public static async Task<TResult[]> ParallelForEachAsync<TSource, TResult>(
        this IEnumerable<TSource> source,
        Func<TSource, CancellationToken, ValueTask<TResult>> body,
        CancellationToken cancellationToken)
    {
        return await ParallelForEachAsync(source, body, 0, cancellationToken);
    }

    public static async Task<TResult[]> ParallelForEachAsync<TSource, TResult>(
        this IEnumerable<TSource> source,
        Func<TSource, ValueTask<TResult>> body,
        int degreeOfParallelism,
        CancellationToken cancellationToken)
    {
        return await ParallelForEachAsync(source, async (x, ct) => await body(x), degreeOfParallelism, cancellationToken);
    }

    public static async Task<TResult[]> ParallelForEachAsync<TSource, TResult>(
        this IEnumerable<TSource> source,
        Func<TSource, CancellationToken, ValueTask<TResult>> body,
        int degreeOfParallelism,
        CancellationToken cancellationToken)
    {
        if (degreeOfParallelism == 1)
        {
            return await ForEachAsync(source, body, cancellationToken);
        }

        var parallelOptions = new ParallelOptions
        {
            MaxDegreeOfParallelism = degreeOfParallelism,
            CancellationToken = cancellationToken
        };

        return await ParallelForEachAsync(source, parallelOptions, body);
    }

    public static async Task<TResult[]> ParallelForEachAsync<TSource, TResult>(
        this IEnumerable<TSource> source,
        ParallelOptions parallelOptions,
        Func<TSource, CancellationToken, ValueTask<TResult>> body)
    {
        var sourceWithIndexes = source.Select((item, index) => (item, index));

        if (source.TryGetNonEnumeratedCount(out int sourceCount))
        {
            var results = new TResult[sourceCount];

            await Parallel.ForEachAsync(
                sourceWithIndexes,
                parallelOptions,
                async (entry, ct) =>
                {
                    results[entry.index] = await body(entry.item, ct);
                });

            return results;
        }
        else
        {
            var results = new List<TResult>();

            await Parallel.ForEachAsync(
                sourceWithIndexes,
                parallelOptions,
                async (entry, ct) =>
                {
                    var result = await body(entry.item, ct);

                    lock (results)
                    {
                        if (entry.index >= results.Count)
                        {
                            results.AddRange(Enumerable.Repeat(default(TResult)!, entry.index - results.Count + 1));
                        }

                        results[entry.index] = result;
                    }
                });

            return results.ToArray();
        }
    }

    /*
    Previous function implementation on
    // https://stackoverflow.com/questions/30907650/foreachasync-with-result
    did it somewhat differently:
    .ContinueWith(
        t =>
            {
                var tcs = new TaskCompletionSource<TResult[]>();

                switch (t.Status)
                {
                    case TaskStatus.RanToCompletion:
                        lock (results) tcs.SetResult(results.ToArray()); break;
                    case TaskStatus.Faulted:
                        tcs.SetException(t.Exception!.InnerExceptions); break;
                    case TaskStatus.Canceled:
                        tcs.SetCanceled(new TaskCanceledException(t).CancellationToken); break;
                    default:
                        throw new Exception("Should not reach here");
                }

                ArgumentExceptionExtensions.ThrowIf(tcs.Task.IsCompleted is not true);

                return tcs.Task;
            },
        parallelOptions.CancellationToken,
        TaskContinuationOptions.DenyChildAttach | TaskContinuationOptions.ExecuteSynchronously,
        TaskScheduler.Default)
    .Unwrap();
    */

    // took SingleOrDefaultIfMultiple from https://stackoverflow.com/questions/3185067/singleordefault-throws-an-exception-on-more-than-one-element

    /// <summary>
    /// Returns the only element of a sequence, or a default value if the sequence is empty or contains more than one element.
    /// </summary>
    public static TSource? SingleOrDefaultIfMultiple<TSource>(this IEnumerable<TSource> source)
    {
        var elements = source.Take(2).ToArray();

        return
            elements.Length == 1
            ? elements[0]
            : default;
    }

    /// <summary>
    /// Returns the only element of a sequence, or a default value if the sequence is empty or contains more than one element.
    /// </summary>
    public static TSource? SingleOrDefaultIfMultiple<TSource>(this IEnumerable<TSource> source, Func<TSource, bool> predicate)
    {
        return source.Where(predicate).SingleOrDefaultIfMultiple();
    }

    /// <summary>
    /// Returns the only element of a sequence, or a default value if the sequence is empty or contains more than one element.
    /// </summary>
    public static TSource? SingleOrDefaultIfMultiple<TSource>(this IQueryable<TSource> source)
    {
        var elements = source.Take(2).ToArray();

        return
            elements.Length == 1
            ? elements[0]
            : default;
    }

    /// <summary>
    /// Returns the only element of a sequence, or a default value if the sequence is empty or contains more than one element.
    /// </summary>
    public static TSource? SingleOrDefaultIfMultiple<TSource>(this IQueryable<TSource> source, Expression<Func<TSource, bool>> predicate)
    {
        return source.Where(predicate).SingleOrDefaultIfMultiple();
    }
}