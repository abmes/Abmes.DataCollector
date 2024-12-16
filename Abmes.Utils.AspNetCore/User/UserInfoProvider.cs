using Abmes.Utils.User;
using Microsoft.AspNetCore.Http;

namespace Abmes.Utils.AspNetCore.User;

public class UserInfoProvider(
    IHttpContextAccessor httpContextAccessor)
    : IUserInfoProvider
{
    public string GetUserId()
    {
        var httpContext = httpContextAccessor.HttpContext;
        ArgumentNullException.ThrowIfNull(httpContext);

        var userId = httpContext.User.Claims.Where(x => x.Type.Equals("sub")).FirstOrDefault()?.Value;
        ArgumentException.ThrowIfNullOrEmpty(userId);

        return userId;
    }

    public UserInfo GetUserInfo()
    {
        var httpContext = httpContextAccessor.HttpContext;
        ArgumentNullException.ThrowIfNull(httpContext);

        var userId = httpContext.User.Claims.Where(x => x.Type.Equals("sub")).FirstOrDefault()?.Value;
        ArgumentException.ThrowIfNullOrEmpty(userId);

        var userFirstName = httpContext.User.Claims.Where(x => x.Type.Equals("given_name")).FirstOrDefault()?.Value;
        ArgumentException.ThrowIfNullOrEmpty(userFirstName);

        var userMiddleName = httpContext.User.Claims.Where(x => x.Type.Equals("middle_name")).FirstOrDefault()?.Value;

        var userLastName = httpContext.User.Claims.Where(x => x.Type.Equals("family_name")).FirstOrDefault()?.Value;
        ArgumentException.ThrowIfNullOrEmpty(userLastName);

        var userIsPowerUser = httpContext.User.Claims.Any(x => x.Type.Equals("role") && string.Equals(x.Value, "admin", StringComparison.InvariantCultureIgnoreCase));

        return new UserInfo(
            UserId: userId,
            UserFirstName: userFirstName,
            UserMiddleName: userMiddleName,
            UserLastName: userLastName,
            UserIsPowerUser: userIsPowerUser);
    }
}
