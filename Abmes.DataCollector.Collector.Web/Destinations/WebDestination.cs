using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Abmes.DataCollector.Collector.Common.Configuration;
using Abmes.DataCollector.Collector.Common.Misc;
using Abmes.DataCollector.Utils;
using IdentityModel.Client;
using Newtonsoft.Json;

namespace Abmes.DataCollector.Collector.Web.Destinations
{
    public class WebDestination : IWebDestination
    {
        public DestinationConfig DestinationConfig { get; }

        private readonly IIdentityServiceHttpRequestConfigurator _identityServiceHttpRequestConfigurator;

        public WebDestination(
            DestinationConfig destinationConfig,
            IIdentityServiceHttpRequestConfigurator identityServiceHttpRequestConfigurator)
        {
            DestinationConfig = destinationConfig;
            _identityServiceHttpRequestConfigurator = identityServiceHttpRequestConfigurator;
        }

        public async Task CollectAsync(string collectUrl, IEnumerable<KeyValuePair<string, string>> collectHeaders, string dataCollectionName, string fileName, TimeSpan timeout, bool finishWait, int tryNo, CancellationToken cancellationToken)
        {
            var endpointUrl = GetEndpointUrl(DestinationConfig.CollectPostEndpoint, dataCollectionName, fileName);

            await HttpUtils.SendAsync(endpointUrl, HttpMethod.Post,
                collectUrl, collectHeaders, null, timeout,
                request => _identityServiceHttpRequestConfigurator.ConfigAsync(request, DestinationConfig.IdentityServiceClientInfo, cancellationToken),
                cancellationToken);
        }

        public async Task<IEnumerable<string>> GetDataCollectionFileNamesAsync(string dataCollectionName, CancellationToken cancellationToken)
        {
            var endpointUrl = GetEndpointUrl(DestinationConfig.FileNamesGetEndpoint, dataCollectionName, null);

            var json =
                await HttpUtils.SendAsync(endpointUrl, HttpMethod.Get,
                    accept: "application/json",
                    requestConfiguratorTask: request => _identityServiceHttpRequestConfigurator.ConfigAsync(request, DestinationConfig.IdentityServiceClientInfo, cancellationToken),
                    cancellationToken: cancellationToken);

            return JsonConvert.DeserializeObject<IEnumerable<string>>(json);
        }

        public async Task GarbageCollectDataCollectionFileAsync(string dataCollectionName, string fileName, CancellationToken cancellationToken)
        {
            var endpointUrl = GetEndpointUrl(DestinationConfig.GarbageCollectFilePostEndpoint, dataCollectionName, fileName);

            await HttpUtils.SendAsync(endpointUrl, HttpMethod.Post,
                requestConfiguratorTask: request => _identityServiceHttpRequestConfigurator.ConfigAsync(request, DestinationConfig.IdentityServiceClientInfo, cancellationToken),
                cancellationToken: cancellationToken);
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

        public bool CanGarbageCollect()
        {
            return 
                (!string.IsNullOrEmpty(DestinationConfig.Root)) && 
                (!string.IsNullOrEmpty(DestinationConfig.FileNamesGetEndpoint)) &&
                (!string.IsNullOrEmpty(DestinationConfig.GarbageCollectFilePostEndpoint));
        }
    }
}
