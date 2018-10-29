using System;
using System.Collections.Generic;
using System.Text;

namespace Abmes.DataCollector.Vault.Service.Configuration
{
    public class IdentityServerAuthenticationSettings
    {
        public string Authority { get; set; }
        public string ApiName { get; set; }
        public string ApiSecret { get; set; }
    }
}
