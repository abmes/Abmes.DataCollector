using System;
using System.Collections.Generic;
using System.Text;

namespace Abmes.DataCollector.Collector.Common.Configuration
{
    public delegate IIdentityServiceClientInfo IIdentityServiceClientInfoFactory(string url, string clientId, string clientSecret, string scope, string userName, string userPassword);
}
