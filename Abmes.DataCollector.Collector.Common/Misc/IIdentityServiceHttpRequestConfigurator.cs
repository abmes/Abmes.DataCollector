using Abmes.DataCollector.Collector.Common.Configuration;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Abmes.DataCollector.Collector.Common.Misc
{
    public interface IIdentityServiceHttpRequestConfigurator
    {
        void Config(HttpRequestMessage request, string identityServiceAccessToken, CancellationToken cancellationToken);
        Task ConfigAsync(HttpRequestMessage request, IIdentityServiceClientInfo identityServiceClientInfo, CancellationToken cancellationToken);
        Task<string> GetIdentityServiceAccessTokenAsync(IIdentityServiceClientInfo identityServiceClientInfo, CancellationToken cancellationToken);
    }
}
