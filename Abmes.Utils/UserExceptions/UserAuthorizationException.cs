namespace Abmes.Utils.UserExceptions;

public class UserAuthorizationException(string message) : UserException(message)
{
    public UserAuthorizationException()
        : this("User has no rights for this operation")
    {
    }
}