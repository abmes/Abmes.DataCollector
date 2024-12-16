using Abmes.Utils.Net;
using Abmes.Utils.UserExceptions;
using Microsoft.AspNetCore.Http;

namespace Abmes.Utils.AspNetCore.ExceptionHandling;

public class UserExceptionHandlingMiddleware(
    RequestDelegate next)
{
    public async Task InvokeAsync(HttpContext context /* other scoped dependencies */)
    {
        try
        {
            await next(context);
        }
        catch (UserException exception)
        {
            await ExceptionHandlingMiddlewareHelper.ReturnUserErrorAsync(
                context,
                UserExceptionHttpHelper.GetUserExceptionHttpStatusCode(exception),
                exception.Message);
        }
    }
}
