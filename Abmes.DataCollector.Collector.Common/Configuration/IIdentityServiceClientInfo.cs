using System;
using System.Collections.Generic;
using System.Text;

namespace Abmes.DataCollector.Collector.Common.Configuration
{
    public interface IIdentityServiceClientInfo
    {
        string Url { get; }
        string ClientId { get; }
        string ClientSecret { get; }
        string Scope { get; }
        string UserName { get; }
        string UserPassword { get; }
    }
}
