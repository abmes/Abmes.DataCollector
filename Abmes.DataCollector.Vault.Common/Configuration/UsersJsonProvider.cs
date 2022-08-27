using System.Text.Json;

namespace Abmes.DataCollector.Vault.Configuration
{
    public class UsersJsonProvider : IUsersJsonProvider
    {
        public IEnumerable<User> GetUsers(string json)
        {
            var options = new JsonSerializerOptions() { PropertyNameCaseInsensitive = true };
            return JsonSerializer.Deserialize<IEnumerable<User>>(json, options);
        }
    }
}
