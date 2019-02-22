using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Abmes.DataCollector.Collector.Common.Configuration;
using Abmes.DataCollector.Utils;
using IdentityModel.Client;
using Newtonsoft.Json;

namespace Abmes.DataCollector.Collector.Web.Destinations
{
    public class WebDestination : IWebDestination
    {
        public DestinationConfig DestinationConfig { get; }

        public WebDestination(
            DestinationConfig destinationConfig)
        {
            DestinationConfig = destinationConfig;
        }

        public async Task CollectAsync(string collectUrl, IEnumerable<KeyValuePair<string, string>> collectHeaders, string dataCollectionName, string fileName, TimeSpan timeout, bool finishWait, int tryNo, CancellationToken cancellationToken)
        {
            var endpointUrl = GetEndpointUrl(DestinationConfig.CollectPostEndpoint, dataCollectionName, fileName);

            await SendEndpointRequestAsync(endpointUrl, dataCollectionName, fileName, HttpMethod.Post, collectUrl, null, collectHeaders, timeout, cancellationToken);
        }

        public async Task<IEnumerable<string>> GetDataCollectionFileNamesAsync(string dataCollectionName, CancellationToken cancellationToken)
        {
            var endpointUrl = GetEndpointUrl(DestinationConfig.FileNamesGetEndpoint, dataCollectionName, null);
            if (string.IsNullOrEmpty(endpointUrl))
            {
                return Enumerable.Empty<string>();
            }

            var json = await SendEndpointRequestAsync(endpointUrl, dataCollectionName, null, HttpMethod.Get, null, "application/json", null, null, cancellationToken);

            return JsonConvert.DeserializeObject<IEnumerable<string>>(json);
        }

        public async Task GarbageCollectDataCollectionFileAsync(string dataCollectionName, string fileName, CancellationToken cancellationToken)
        {
            var endpointUrl = GetEndpointUrl(DestinationConfig.GarbageCollectFilePostEndpoint, dataCollectionName, fileName);
            if (string.IsNullOrEmpty(endpointUrl))
            {
                return;
            }

            await SendEndpointRequestAsync(endpointUrl, dataCollectionName, fileName, HttpMethod.Post, null, null, null, null, cancellationToken);
        }

        private async Task<string> SendEndpointRequestAsync(string endpointUrl, string dataCollectionName, string fileName, HttpMethod httpMethod, string content, string acceptHeader, IEnumerable<KeyValuePair<string, string>> headers, TimeSpan? timeout, CancellationToken cancellationToken)
        {
            using (var httpClient = new HttpClient())
            {
                if (timeout.HasValue)
                {
                    httpClient.Timeout = timeout.Value;
                }

                using (var request = new HttpRequestMessage(httpMethod, endpointUrl))
                {
                    request.Headers.AddValues(headers);

                    if (!string.IsNullOrEmpty(acceptHeader))
                    {
                        request.Headers.Accept.Add(new MediaTypeWithQualityHeaderValue(acceptHeader));
                    }

                    await SetRequestAuthorizationAsync(request, cancellationToken);

                    if (!string.IsNullOrEmpty(content))
                    {
                        request.Content = new StringContent(content);
                    }

                    using (var response = await httpClient.SendAsync(request, cancellationToken))
                    {
                        await response.CheckSuccessAsync();

                        return await response.Content.ReadAsStringAsync();
                    }
                }
            }
        }

        private string GetEndpointUrl(string endpoint, string dataCollectionName, string fileName)
        {
            if (string.IsNullOrEmpty(DestinationConfig.Root) || string.IsNullOrEmpty(endpoint))
            {
                return null;
            }

            var result = DestinationConfig.Root.TrimEnd('/') + "/" + endpoint.TrimStart('/');

            result = result.Replace("[DataCollectionName]", dataCollectionName);

            if (!string.IsNullOrEmpty(fileName))
            {
                result = result.Replace("[FileName]", fileName);
            }

            return result;
        }

        private async Task SetRequestAuthorizationAsync(HttpRequestMessage request, CancellationToken cancellationToken)
        {
            var accessToken = await GetIdentityServiceAccessTokenAsync(cancellationToken);

            if (!string.IsNullOrEmpty(accessToken))
            {
                request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", accessToken);
            }
        }

        private async Task<string> GetIdentityServiceAccessTokenAsync(CancellationToken cancellationToken)
        {
            if (string.IsNullOrEmpty(DestinationConfig.IdentityServiceUrl))
            {
                return null;
            }

            var tokenRequest = new PasswordTokenRequest
            {
                Address = DestinationConfig.IdentityServiceUrl.TrimEnd('/') + "/connect/token",

                ClientId = DestinationConfig.IdentityServiceClientId,
                ClientSecret = DestinationConfig.IdentityServiceClientSecret,
                Scope = DestinationConfig.IdentityServiceScope,

                UserName = DestinationConfig.LoginName,
                Password = DestinationConfig.LoginSecret
            };

            using (var httpClient = new HttpClient())
            {
                var response = await httpClient.RequestPasswordTokenAsync(tokenRequest, cancellationToken);

                return response.AccessToken;
            }
        }
    }
}
