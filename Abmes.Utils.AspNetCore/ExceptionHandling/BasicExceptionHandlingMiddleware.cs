using Microsoft.AspNetCore.Http;
using System.Net;

namespace Abmes.Utils.AspNetCore.ExceptionHandling;

public class BasicExceptionHandlingMiddleware(
    RequestDelegate next)
{
    public async Task InvokeAsync(HttpContext context /* other scoped dependencies */)
    {
        try
        {
            await next(context);
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
