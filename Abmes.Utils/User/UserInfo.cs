namespace Abmes.Utils.User;

public record UserInfo
(
    string UserId,
    string UserFirstName,
    string? UserMiddleName,
    string UserLastName,
    bool UserIsPowerUser
);
