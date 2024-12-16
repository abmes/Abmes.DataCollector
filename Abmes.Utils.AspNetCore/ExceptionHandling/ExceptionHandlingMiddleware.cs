using Microsoft.AspNetCore.Http;

namespace Abmes.Utils.AspNetCore.ExceptionHandling;

public class ExceptionHandlingMiddleware(
    RequestDelegate next,
    ExceptionHandlingMiddlewareOptions options)
{
    public async Task InvokeAsync(HttpContext context /* other scoped dependencies */)
    {
        try
        {
            await next(context);
        }
        catch (Exception exception) when (options.CanHandleExceptionFunc(exception))
        {
            if (!await options.TryHandleExceptionAsyncFunc(exception, context))
            {
                throw;
            }
        }
    }
}
