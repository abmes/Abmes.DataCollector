using System.Diagnostics.CodeAnalysis;
using System.Runtime.CompilerServices;

namespace Abmes.Utils;

public static class ArgumentExceptionExtensions
{
    public static void ThrowIf([DoesNotReturnIf(true)] bool condition, [CallerArgumentExpression(nameof(condition))] string? conditionText = null)
    {
        if (condition)
        {
            throw new ArgumentException(conditionText);
        }
    }

    public static void ThrowIfNot([DoesNotReturnIf(false)] bool condition, [CallerArgumentExpression(nameof(condition))] string? conditionText = null)
    {
        if (!condition)
        {
            throw new ArgumentException($"'{conditionText}' is not true");
        }
    }
}
