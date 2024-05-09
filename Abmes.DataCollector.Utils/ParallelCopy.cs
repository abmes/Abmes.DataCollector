using System.Buffers;
using System.Threading.Channels;

namespace Abmes.DataCollector.Utils;

public static class ParallelCopy
{
    public static async Task CopyAsync(
        Func<Memory<byte>, CancellationToken, ValueTask<int>> copyReadAsyncFunc,
        Func<ReadOnlyMemory<byte>, CancellationToken, ValueTask> copyWriteAsyncFunc,
        int bufferSize,
        CancellationToken cancellationToken)
    {
        ArgumentExceptionExtensions.ThrowIf(bufferSize <= 0);

        using var mem0 = MemoryPool<byte>.Shared.Rent(bufferSize);
        using var mem1 = MemoryPool<byte>.Shared.Rent(bufferSize);
        using var mem2 = MemoryPool<byte>.Shared.Rent(bufferSize);
        using var mem3 = MemoryPool<byte>.Shared.Rent(bufferSize);

        var buffers = new[] { mem0.Memory, mem1.Memory, mem2.Memory, mem3.Memory };

        // reader and writer should each have its own buffer that is not in the channel and should not use the same buffer thus the -2
        var channel = Channel.CreateBounded<(int BufferIndex, int ByteCount)>(buffers.Length-2);

        var readTask = Task.Run(
            async () =>
            {
                var bufferIndex = 0;
                while (true)
                {
                    var bytesRead = await CopyUtils.FillBufferAsync(buffers[bufferIndex], copyReadAsyncFunc, cancellationToken);

                    if (bytesRead == 0)
                    {
                        break;
                    }

                    await channel.Writer.WriteAsync((bufferIndex, bytesRead), cancellationToken);
                
                    bufferIndex = (bufferIndex + 1) % buffers.Length;
                }

                channel.Writer.Complete();
            },
            cancellationToken);

        var writeTask = Task.Run(
            async () =>
            {
                await foreach (var item in channel.Reader.ReadAllAsync(cancellationToken))
                {
                    await copyWriteAsyncFunc(buffers[item.BufferIndex][..item.ByteCount], cancellationToken);
                }
            },
            cancellationToken);

        await Task.WhenAll(readTask, writeTask);
    }
}
