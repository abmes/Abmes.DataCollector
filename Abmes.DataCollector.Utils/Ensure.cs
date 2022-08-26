using System.Diagnostics.CodeAnalysis;
using System.Runtime.CompilerServices;

namespace Abmes.DataCollector.Utils;

public static class Ensure
{
    [return: NotNull]
    public static T NotNull<T>([NotNull] T? argument, [CallerArgumentExpression("argument")] string? paramName = null)
    {
        ArgumentNullException.ThrowIfNull(argument, paramName);
        return argument;
    }

    [return: NotNull]
    public static string NotNullOrEmpty([NotNull] string? argument, [CallerArgumentExpression("argument")] string? paramName = null)
    {
        ArgumentExceptionExtensions.ThrowIfNullOrEmpty(argument, paramName);
        return argument;
    }
}
