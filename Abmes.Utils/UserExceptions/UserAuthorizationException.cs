namespace Abmes.Utils.UserExceptions;

public class UserAuthorizationException : UserException
{
    public UserAuthorizationException()
        : base("User has no rights for this operation")
    {
    }

    public UserAuthorizationException(string message)
        : base(message)
    {
    }
}
