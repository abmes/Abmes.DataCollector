using System.Buffers;
using System.Security.Cryptography;

namespace Abmes.DataCollector.Utils;

public static class CopyUtils
{
    private static async Task<int> FillBufferAsync(
        Memory<byte> buffer,
        Func<Memory<byte>, CancellationToken, Task<int>> readTask,
        CancellationToken cancellationToken)
    {
        int bytesRead;
        int totalBytesRead = 0;

        do
        {
            bytesRead = await readTask(buffer[totalBytesRead..], cancellationToken);

            if (bytesRead > 0)
            {
                totalBytesRead += bytesRead;
            }
        }
        while ((bytesRead > 0) && (totalBytesRead < buffer.Length));

        return totalBytesRead;
    }

    public static async Task<int> ReadStreamMaxBufferAsync(
        Memory<byte> buffer,
        Stream stream,
        CancellationToken cancellationToken)
    {
        return await FillBufferAsync(
            buffer,
            async (buf, ct) => await stream.ReadAsync(buf, ct),
            cancellationToken);
    }

    public static byte[] GetMD5Hash(ReadOnlyMemory<byte> buffer)
    {
        var hasher = GetMD5Hasher();
        AppendMDHasherData(hasher, buffer);
        return GetMD5Hash(hasher);
    }

    public static string? GetMD5HashString(ReadOnlyMemory<byte> buffer)
    {
        return GetMD5HashString(GetMD5Hash(buffer));
    }

    public static async Task<string?> GetMD5HashStringAsync(Stream stream, int bufferSize, CancellationToken cancellationToken)
    {
        var hasher = GetMD5Hasher();

        using var mem = MemoryPool<byte>.Shared.Rent(bufferSize);
        var buffer = mem.Memory;
        while (true)
        {
            var bytesRead = await ReadStreamMaxBufferAsync(buffer, stream, cancellationToken);

            if (bytesRead == 0)
            {
                break;
            }

            AppendMDHasherData(hasher, buffer[..bytesRead]);
        }

        return GetMD5HashString(hasher);
    }

    public static IncrementalHash GetMD5Hasher()
    {
        return IncrementalHash.CreateHash(HashAlgorithmName.MD5);
    }

    public static void AppendMDHasherData(IncrementalHash hasher, ReadOnlyMemory<byte> buffer)
    {
        hasher.AppendData(buffer.Span);
    }

    public static byte[] GetMD5Hash(IncrementalHash hasher)
    {
        return hasher.GetHashAndReset();
    }

    public static string? GetMD5HashString(IncrementalHash hasher)
    {
        return GetMD5HashString(GetMD5Hash(hasher));
    }

    public static string? GetMD5HashString(byte[]? md5Hash)
    {
        if ((md5Hash is null) || (md5Hash.Length == 0))
        {
            return null;
        }

        return Convert.ToBase64String(md5Hash, 0, 16);
    }
}
