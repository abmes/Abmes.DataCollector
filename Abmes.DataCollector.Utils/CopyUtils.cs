using System.Buffers;
using System.Threading.Channels;

namespace Abmes.DataCollector.Utils;

public static class CopyUtils
{
    public static async Task ParallelCopyAsync(
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
        var channel = Channel.CreateBounded<(int BufferIndex, int ByteCount)>(buffers.Length - 2);

        var readTask = Task.Run(
            async () =>
            {
                var bufferIndex = 0;
                while (true)
                {
                    var bytesRead = await FillBufferAsync(buffers[bufferIndex], copyReadAsyncFunc, cancellationToken);

                    if (bytesRead > 0)
                    {
                        await channel.Writer.WriteAsync((bufferIndex, bytesRead), cancellationToken);
                    }

                    if (bytesRead < buffers[bufferIndex].Length)
                    {
                        break;
                    }

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

    public static async ValueTask<int> FillBufferAsync(
        Memory<byte> buffer,
        Func<Memory<byte>, CancellationToken, ValueTask<int>> readTask,
        CancellationToken cancellationToken)
    {
        var totalBytesRead = 0;

        while (totalBytesRead < buffer.Length)
        {
            var bytesRead = await readTask(buffer[totalBytesRead..], cancellationToken);

            if (bytesRead == 0)
            {
                break;
            }
            
            totalBytesRead += bytesRead;
        }

        return totalBytesRead;
    }
}
