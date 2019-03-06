using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Abmes.DataCollector.Collector.Common.Configuration;
using IdentityModel.Client;

namespace Abmes.DataCollector.Collector.Common.Misc
{
    public class IdentityServiceHttpRequestConfigurator : IIdentityServiceHttpRequestConfigurator
    {
        public async Task ConfigAsync(HttpRequestMessage request, IIdentityServiceClientInfo identityServiceClientInfo, CancellationToken cancellationToken)
        {
            var accessToken = await GetIdentityServiceAccessTokenAsync(identityServiceClientInfo, cancellationToken);

            if (!string.IsNullOrEmpty(accessToken))
            {
                request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", accessToken);
            }
        }

        private async Task<string> GetIdentityServiceAccessTokenAsync(IIdentityServiceClientInfo identityServiceClientInfo, CancellationToken cancellationToken)
        {
            if (string.IsNullOrEmpty(identityServiceClientInfo.Url))
            {
                return null;
            }

            var tokenRequest = new PasswordTokenRequest
            {
                Address = identityServiceClientInfo.Url.TrimEnd('/') + "/connect/token",

                ClientId = identityServiceClientInfo.ClientId,
                ClientSecret = identityServiceClientInfo.ClientSecret,
                Scope = identityServiceClientInfo.Scope,

                UserName = identityServiceClientInfo.UserName,
                Password = identityServiceClientInfo.UserPassword
            };

            using (var httpClient = new HttpClient())
            {
                var response = await httpClient.RequestPasswordTokenAsync(tokenRequest, cancellationToken);

                return response.AccessToken;
            }
        }
    }
}
