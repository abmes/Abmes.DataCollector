using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Text;

namespace Abmes.DataCollector.Vault.Configuration
{
    public class UsersJsonProvider : IUsersJsonProvider
    {
        public IEnumerable<User> GetUsers(string json)
        {
            return JsonConvert.DeserializeObject<IEnumerable<User>>(json);
        }
    }
}
