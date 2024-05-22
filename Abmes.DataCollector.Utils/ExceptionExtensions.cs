namespace Abmes.DataCollector.Utils;

public static class ExceptionExtensions
{
    private static IEnumerable<Exception> GetInnerExceptionChain(Exception exception)
    {
        // todo: functional (abstract enumerable chain)
        var e = exception;
        while (e is not null)
        {
            yield return e;
            e = e.InnerException;
        }
    }

    public static string GetInnerMessages(this Exception exception)
    {
        return string.Join(Environment.NewLine, GetInnerExceptionChain(exception).Select(e => e.Message));
    }

    public static string GetAggregateMessages(this Exception exception)
    {
        return
            exception is AggregateException e
            ? string.Join(
                Environment.NewLine,
                e.InnerExceptions.Select(x => x.GetAggregateMessages()))
            : exception.GetInnerMessages();
    }
}
