using System.Text.Json;

namespace Abmes.DataCollector.Vault.Configuration;

public class UsersJsonProvider : IUsersJsonProvider
{
    private static readonly JsonSerializerOptions _jsonSerializerOptions = new() { PropertyNameCaseInsensitive = true };

    public IEnumerable<User> GetUsers(string json)
    {
        return JsonSerializer.Deserialize<IEnumerable<User>>(json, _jsonSerializerOptions) ?? [];
    }
}
