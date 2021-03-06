﻿using System;
using System.Collections.Generic;
using System.IO;
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
using System.Text.Json;

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

        public async Task CollectAsync(string collectUrl, IEnumerable<KeyValuePair<string, string>> collectHeaders, IIdentityServiceClientInfo collectIdentityServiceClientInfo, string dataCollectionName, string fileName, TimeSpan timeout, bool finishWait, int tryNo, CancellationToken cancellationToken)
        {
            var endpointUrl = GetEndpointUrl(DestinationConfig.CollectPostEndpoint, dataCollectionName, fileName);

            if (string.IsNullOrEmpty(endpointUrl))
            {
                return;
            }

            await HttpUtils.SendAsync(endpointUrl, HttpMethod.Post,
                collectUrl, null, collectHeaders, null, timeout,
                request => _identityServiceHttpRequestConfigurator.ConfigAsync(request, DestinationConfig.IdentityServiceClientInfo, cancellationToken),
                cancellationToken: cancellationToken);
        }

        public async Task<IEnumerable<string>> GetDataCollectionFileNamesAsync(string dataCollectionName, CancellationToken cancellationToken)
        {
            var endpointUrl = GetEndpointUrl(DestinationConfig.FileNamesGetEndpoint, dataCollectionName, null);

            var json =
                await HttpUtils.GetStringAsync(endpointUrl, HttpMethod.Get,
                    accept: "application/json",
                    requestConfiguratorTask: request => _identityServiceHttpRequestConfigurator.ConfigAsync(request, DestinationConfig.IdentityServiceClientInfo, cancellationToken),
                    cancellationToken: cancellationToken);

            var options = new JsonSerializerOptions() { PropertyNameCaseInsensitive = true };
            return JsonSerializer.Deserialize<IEnumerable<string>>(json, options);
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

        public Task<bool> AcceptsFileAsync(string dataCollectionName, string name, long? size, string md5, CancellationToken cancellationToken)
        {
            return Task.FromResult(true);
        }

        public async Task PutFileAsync(string dataCollectionName, string fileName, Stream content, CancellationToken cancellationToken)
        {
            var endpointUrl = GetEndpointUrl(DestinationConfig.CollectPostEndpoint, dataCollectionName, fileName);

            if (string.IsNullOrEmpty(endpointUrl))
            {
                return;
            }

            await HttpUtils.SendAsync(endpointUrl, HttpMethod.Post,
                null, content, null, null, null,
                request => _identityServiceHttpRequestConfigurator.ConfigAsync(request, DestinationConfig.IdentityServiceClientInfo, cancellationToken),
                cancellationToken: cancellationToken);
        }
    }
}
