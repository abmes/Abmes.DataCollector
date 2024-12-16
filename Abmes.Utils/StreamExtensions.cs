using System.Runtime.CompilerServices;
using System.Text;

namespace Abmes.Utils;

public static class StreamExtensions
{
    // todo: make a pull request to contribute this code to the standard Stream.CopyToAsync() code in .net
    public static async Task CopyToParallelAsync(this Stream source, Stream destination, int bufferSize, CancellationToken cancellationToken)
    {
        ArgumentExceptionExtensions.ThrowIf(source.CanRead is false);
        ArgumentExceptionExtensions.ThrowIf(destination.CanWrite is false);

        await CopyUtils.ParallelCopyAsync(source.ReadAsync, destination.WriteAsync, bufferSize, cancellationToken);
    }

    public static IEnumerable<string> ReadAllLines(this Stream stream)
    {
        return stream.ReadAllLines(Encoding.UTF8);
    }

    public static IEnumerable<string> ReadAllLines(this Stream stream, Encoding encoding)
    {
        // when using this method please be carefull not to dispose the stream before the end of the enumeration
        using var reader = new StreamReader(stream, encoding);

        while (true)
        {
            var line = reader.ReadLine();

            if (line is null)
            {
                yield break;
            }

            yield return line;
        }
    }

    public static async IAsyncEnumerable<string> ReadAllLinesAsync(this Stream stream, [EnumeratorCancellation] CancellationToken cancellationToken)
    {
        await foreach (var line in stream.ReadAllLinesAsync(Encoding.UTF8, cancellationToken))
        {
            yield return line;
        }
    }

    public static async IAsyncEnumerable<string> ReadAllLinesAsync(this Stream stream, Encoding encoding, [EnumeratorCancellation] CancellationToken cancellationToken)
    {
        // when using this method please be carefull not to dispose the stream before the end of the enumeration
        using var reader = new StreamReader(stream, encoding);

        while (true)  // EndOfStream is synchronous - do not use it!
        {
            var line = await reader.ReadLineAsync(cancellationToken);

            if (line is null)
            {
                yield break;
            }

            yield return line;
        }
    }
}
