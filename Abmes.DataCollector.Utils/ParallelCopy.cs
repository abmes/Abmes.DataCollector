using System.Buffers;

namespace Abmes.DataCollector.Utils;

public static class ParallelCopy
{
    public static async Task CopyAsync(
        Func<Memory<byte>, CancellationToken, Func<Task<int>>> copyReadTaskFunc,
        Func<ReadOnlyMemory<byte>, CancellationToken, Func<Task>> copyWriteTaskFunc,
        int bufferSize,
        CancellationToken cancellationToken)
    {
        ArgumentExceptionExtensions.ThrowIf(bufferSize <= 0);

        using var mem0 = MemoryPool<byte>.Shared.Rent(bufferSize);
        using var mem1 = MemoryPool<byte>.Shared.Rent(bufferSize);

        var buffers = new[] { mem0.Memory, mem1.Memory };

        var bufferIndex = 0;
        var writeTask = Task.CompletedTask;
        while (true)
        {
            // .net 6 changed some behavior. now Task.Run(func) is needed. Skipping it serializes the execution
            var readTask = Task.Run(copyReadTaskFunc(buffers[bufferIndex], cancellationToken), cancellationToken);

            await Task.WhenAll(readTask, writeTask);

            int bytesRead = readTask.Result;

            if (bytesRead == 0)
            {
                break;
            }

            writeTask = Task.Run(copyWriteTaskFunc(buffers[bufferIndex].Slice(0, bytesRead), cancellationToken), cancellationToken);
            // careful - do not capture this variable by ref in a lambda as its value changes
            bufferIndex = 1 - bufferIndex;
        }
    }
}
