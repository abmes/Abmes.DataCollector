using Microsoft.AspNetCore.Http;

namespace Abmes.Utils.AspNetCore.ExceptionHandling;

public record ExceptionHandlingMiddlewareOptions
(
    Func<Exception, bool> CanHandleExceptionFunc,
    Func<Exception, HttpContext, Task<bool>> TryHandleExceptionAsyncFunc
);
