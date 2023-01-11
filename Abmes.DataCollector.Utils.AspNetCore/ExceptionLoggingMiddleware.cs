using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;

namespace Abmes.DataCollector.Utils.AspNetCore;

public class ExceptionLoggingMiddleware
{
    private readonly RequestDelegate _next;
    private readonly ILoggerFactory _loggerFactory;

    public ExceptionLoggingMiddleware(
        RequestDelegate next,
        ILoggerFactory loggerFactory)
    {
        _next = next;
        _loggerFactory = loggerFactory;
    }

    public async Task InvokeAsync(HttpContext context /* other scoped dependencies */)
    {
        try
        {
            await _next(context);
        }
        catch (Exception exception) when (Log(exception))
        {
            // should never come here as Log() always returns false
        }
    }

    private bool Log(Exception exception)
    {
        _loggerFactory.CreateLogger("Errors").LogError(0, exception, exception.GetAggregateMessages());
        return false;
    }
}
