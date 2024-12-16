namespace Abmes.Utils.User;

public interface IUserInfoProvider
{
    string GetUserId();
    UserInfo GetUserInfo();
}
