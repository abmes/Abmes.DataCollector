using Abmes.DataCollector.Common.Configuration;
using Abmes.DataCollector.Vault.Configuration;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Abmes.DataCollector.Vault.Common.Configuration
{
    public class UsersProvider : IUsersProvider
    {
        private const string UsersConfigName = "Users.json";

        private readonly IUsersJsonProvider _usersJsonProvider;
        private readonly IConfigProvider _configProvider;

        public UsersProvider(
            IUsersJsonProvider usersJsonProvider,
            IConfigProvider configProvider)
        {
            _usersJsonProvider = usersJsonProvider;
            _configProvider = configProvider;
        }

        public async Task<IEnumerable<User>> GetUsersAsync(CancellationToken cancellationToken)
        {
            var json = await _configProvider.GetConfigContentAsync(UsersConfigName, cancellationToken);
            return _usersJsonProvider.GetUsers(json);
        }
    }
}
