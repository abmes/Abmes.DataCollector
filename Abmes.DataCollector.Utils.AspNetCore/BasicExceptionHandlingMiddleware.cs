using Microsoft.AspNetCore.Http;
using System.Net;

namespace Abmes.DataCollector.Utils.AspNetCore;

public class BasicExceptionHandlingMiddleware
{
    private readonly RequestDelegate _next;

    public BasicExceptionHandlingMiddleware(RequestDelegate next)
    {
        _next = next;
    }

    public async Task InvokeAsync(HttpContext context /* other scoped dependencies */)
    {
        try
        {
            await _next(context);
        }
        catch (Exception exception)
        {
            await ExceptionHandlingMiddlewareHelper.ReturnUserErrorAsync(
                context,
                HttpStatusCode.InternalServerError,
                exception.GetAggregateMessages());
        }
    }
}
