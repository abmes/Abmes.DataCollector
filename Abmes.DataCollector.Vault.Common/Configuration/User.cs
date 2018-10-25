using System;
using System.Collections.Generic;
using System.Text;

namespace Abmes.DataCollector.Vault.Configuration
{
    public class User
    {
        public string IdentityUserId { get; }
        public IEnumerable<string> DataCollectionNames { get; set; }

        public User(string identityUserId, IEnumerable<string> dataCollectionNames)
        {
            IdentityUserId = identityUserId;
            DataCollectionNames = dataCollectionNames;
        }
    }
}
