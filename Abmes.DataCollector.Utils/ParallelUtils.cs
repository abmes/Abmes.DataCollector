using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Threading.Tasks.Dataflow;

namespace Abmes.DataCollector.Utils
{
    public static class ParallelUtils
    {
        public static async Task<ActionBlock<T>> ParallelEnumerateAsync<T>(IEnumerable<T> items, CancellationToken cancellationToken, int maxDegreeOfParallelism, Func<T, CancellationToken, Task> func)
        {
            var dataflowBlockOptions = new System.Threading.Tasks.Dataflow.ExecutionDataflowBlockOptions
            {
                MaxDegreeOfParallelism = maxDegreeOfParallelism,
                CancellationToken = cancellationToken
            };

            var workerBlock = new ActionBlock<T>(item => func(item, cancellationToken), dataflowBlockOptions);

            foreach (var item in items)
            {
                workerBlock.Post(item);
            }

            workerBlock.Complete();

            await workerBlock.Completion;

            return workerBlock;
        }

        public static async Task<ActionBlock<T>> ParallelEnumerateAsync<T>(IEnumerable<T> items, CancellationToken cancellationToken, int maxDegreeOfParallelism, Action<T, CancellationToken> action)
        {
            return
                await ParallelEnumerateAsync(items, cancellationToken, maxDegreeOfParallelism,
                (item, ct) =>
                {
                    action(item, ct);
                    return Task.CompletedTask;
                });
        }
    }
}
