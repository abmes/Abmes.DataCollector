using System.Buffers;
using System.Security.Cryptography;

namespace Abmes.DataCollector.Utils;

public static class CopyUtils
{
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
            var bytesRead = await FillBufferAsync(buffer, stream.ReadAsync, cancellationToken);

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
        return (md5Hash is null) || (md5Hash.Length == 0) ? null : Convert.ToBase64String(md5Hash, 0, 16);
    }

    public static async Task<string> GetStreamMD5Async(Stream stream, CancellationToken cancellationToken)
    {
        using var md5 = MD5.Create();
        var hash = await md5.ComputeHashAsync(stream, cancellationToken);

        return Convert.ToBase64String(hash);
    }
}
