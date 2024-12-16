using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;

namespace Abmes.Utils.AspNetCore.ExceptionHandling;

public class ExceptionLoggingMiddleware(
    RequestDelegate next,
    ILoggerFactory loggerFactory)
{
    public async Task InvokeAsync(HttpContext context /* other scoped dependencies */)
    {
        try
        {
            await next(context);
        }
        catch (Exception exception) when (Log(exception))
        {
            // should never come here as Log() always returns false
        }
    }

    private bool Log(Exception exception)
    {
        loggerFactory.CreateLogger("Errors").LogError(0, exception, "{messages}", exception.GetAggregateMessages());
        return false;
    }
}
