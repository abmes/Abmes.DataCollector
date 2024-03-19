using System.Diagnostics.CodeAnalysis;
using System.Runtime.CompilerServices;

namespace Abmes.DataCollector.Utils;

public static class ArgumentExceptionExtensions
{
    public static void ThrowIf([DoesNotReturnIf(true)] bool condition, [CallerArgumentExpression(nameof(condition))] string? conditionText = null)
    {
        if (condition)
        {
            throw new ArgumentException(conditionText);
        }
    }

    // todo: this will be in .net 7 - remove it after migrating to .net 7 or newer
    public static void ThrowIfNullOrEmpty([NotNull] string? argument, [CallerArgumentExpression(nameof(argument))] string? paramName = null)
    {
        if (string.IsNullOrEmpty(argument))
        {
            ThrowNullOrEmptyException(argument, paramName);
        }
    }

    [DoesNotReturn]
    private static void ThrowNullOrEmptyException(string? argument, string? paramName)
    {
        ArgumentNullException.ThrowIfNull(argument, paramName);
        throw new ArgumentException("The value cannot be an empty string.", paramName);
    }
}
