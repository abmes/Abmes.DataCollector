using System;
using System.Collections.Generic;
using System.Text;

namespace Abmes.DataCollector.Collector.Common.Configuration
{
    public class IdentityServiceClientInfo : IIdentityServiceClientInfo
    {
        public string Url { get; }
        public string ClientId { get; }
        public string ClientSecret { get; }
        public string Scope { get; }
        public string UserName { get; }
        public string UserPassword { get; }

        public IdentityServiceClientInfo(string url, string clientId, string clientSecret, string scope, string userName, string userPassword)
        {
            Url = url;
            ClientId = clientId;
            ClientSecret = clientSecret;
            Scope = scope;
            UserName = userName;
            UserPassword = userPassword;
        }
    }
}
