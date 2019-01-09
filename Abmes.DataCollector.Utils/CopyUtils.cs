using System;
using System.Buffers;
using System.Collections.Generic;
using System.Diagnostics.Contracts;
using System.IO;
using System.Security.Cryptography;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Abmes.DataCollector.Utils
{
    public static class CopyUtils
    {
        public static async Task CopyAsync(Func<byte[], CancellationToken, Task<int>> copyReadTask, Func<byte[], int, CancellationToken, Task> copyWriteTask, Int32 bufferSize, CancellationToken cancellationToken)
        {
            Contract.Requires(copyReadTask != null);
            Contract.Requires(copyWriteTask != null);
            Contract.Requires(bufferSize > 0);

            byte[][] buffers = { ArrayPool<byte>.Shared.Rent(bufferSize), ArrayPool<byte>.Shared.Rent(bufferSize) };

            try
            {
                var bufferIndex = 0;
                var writeTask = Task.CompletedTask;
                while (true)
                {
                    var readTask = copyReadTask(buffers[bufferIndex], cancellationToken);

                    await Task.WhenAll(readTask, writeTask);

                    int bytesRead = readTask.Result;

                    if (bytesRead == 0)
                    {
                        break;
                    }

                    writeTask = copyWriteTask(buffers[bufferIndex], bytesRead, cancellationToken);

                    bufferIndex = 1 - bufferIndex;
                }
            }
            finally
            {
                foreach (var b in buffers)
                    ArrayPool<byte>.Shared.Return(b, clearArray: true);
            }
        }

        public static async Task<int> FillBufferAsync(byte[] buffer, Func<byte[], int, CancellationToken, Task<int>> readTask, CancellationToken cancellationToken)
        {
            int bytesRead;
            int totalBytesRead = 0;

            do
            {
                bytesRead = await readTask(buffer, totalBytesRead, cancellationToken);

                if (bytesRead > 0)
                {
                    totalBytesRead += bytesRead;
                }
            }
            while ((bytesRead > 0) && (totalBytesRead < buffer.Length));

            return totalBytesRead;
        }

        public static async Task<int> ReadStreamMaxBufferAsync(byte[] buffer, Stream stream, CancellationToken cancellationToken)
        {
            return await CopyUtils.FillBufferAsync(buffer,
                    async (buf, offset, ct) =>
                    {
                        return await stream.ReadAsync(buf, offset, buf.Length - offset, ct);
                    },
                    cancellationToken
                );
        }

        public static string GetMD5Hash(byte[] buffer, int offset, int count)
        {
            var hasher = GetMD5Hasher();
            AppendMDHasherData(hasher, buffer, offset, count);
            return GetMD5Hash(hasher);
        }

        public static IncrementalHash GetMD5Hasher()
        {
            return IncrementalHash.CreateHash(HashAlgorithmName.MD5);
        }

        public static void AppendMDHasherData(IncrementalHash hasher, byte[] buffer, int offset, int count)
        {
            hasher.AppendData(buffer, offset, count);
        }

        public static string GetMD5Hash(IncrementalHash hasher)
        {
            var byteHash = hasher.GetHashAndReset();
            var md5Hash = GetMD5Hash(byteHash);
            return md5Hash;
        }

        public static string GetMD5Hash(byte[] md5Hash)
        {
            if ((md5Hash == null) || (md5Hash.Length == 0))
            {
                return null;
            }

            return Convert.ToBase64String(md5Hash, 0, 16);
        }
    }
}
