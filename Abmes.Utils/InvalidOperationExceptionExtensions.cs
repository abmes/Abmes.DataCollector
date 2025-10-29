using System.Diagnostics.CodeAnalysis;
using System.Runtime.CompilerServices;

namespace Abmes.Utils;

public static class InvalidOperationExceptionExtensions
{
    public static void ThrowIf([DoesNotReturnIf(true)] bool condition, [CallerArgumentExpression(nameof(condition))] string? conditionText = null)
    {
        if (condition)
        {
            throw new InvalidOperationException(conditionText);
        }
    }

    public static void ThrowIfNot([DoesNotReturnIf(false)] bool condition, [CallerArgumentExpression(nameof(condition))] string? conditionText = null)
    {
        if (!condition)
        {
            throw new InvalidOperationException($"'{conditionText}' is not true");
        }
    }
}
