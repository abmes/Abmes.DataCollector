using Abmes.Utils.UserExceptions;
using System.Net;

namespace Abmes.Utils.Net;

public static class UserExceptionHttpHelper
{
    public static HttpStatusCode GetUserExceptionHttpStatusCode(UserException exception)
    {
        return
            exception switch
            {
                UserAuthenticationException => HttpStatusCode.Unauthorized,
                UserAuthorizationException => HttpStatusCode.Forbidden,
                _ => HttpStatusCode.BadRequest
            };
    }
}
