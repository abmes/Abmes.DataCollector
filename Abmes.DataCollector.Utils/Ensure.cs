using System.Diagnostics.CodeAnalysis;
using System.Runtime.CompilerServices;

namespace Abmes.DataCollector.Utils;

public static class Ensure
{
    [return: NotNull]
    public static T NotNull<T>([NotNull] T? argument, [CallerArgumentExpression(nameof(argument))] string? paramName = null) where T : class
    {
        ArgumentNullException.ThrowIfNull(argument, paramName);
        return argument;
    }

    [return: NotNull]
    public static T NotNull<T>([NotNull] T? argument, [CallerArgumentExpression(nameof(argument))] string? paramName = null) where T : struct
    {
        ArgumentNullException.ThrowIfNull(argument, paramName);
        return argument.Value;
    }

    [return: NotNull]
    public static string NotNullOrEmpty([NotNull] string? argument, [CallerArgumentExpression(nameof(argument))] string? paramName = null)
    {
        ArgumentException.ThrowIfNullOrEmpty(argument, paramName);
        return argument;
    }
}
