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

namespace Abmes.DataCollector.Collector.Console.Destinations
{
    public class ConsoleDestination : IConsoleDestination
    {
        public DestinationConfig DestinationConfig { get; }

        private readonly IIdentityServiceHttpRequestConfigurator _identityServiceHttpRequestConfigurator;

        public ConsoleDestination(
            DestinationConfig destinationConfig,
            IIdentityServiceHttpRequestConfigurator identityServiceHttpRequestConfigurator)
        {
            DestinationConfig = destinationConfig;
            _identityServiceHttpRequestConfigurator = identityServiceHttpRequestConfigurator;
        }

        public async Task CollectAsync(string collectUrl, IEnumerable<KeyValuePair<string, string>> collectHeaders, IIdentityServiceClientInfo collectIdentityServiceClientInfo, string dataCollectionName, string fileName, TimeSpan timeout, bool finishWait, int tryNo, CancellationToken cancellationToken)
        {
            var content =
                    await HttpUtils.GetStringAsync(collectUrl, HttpMethod.Get,
                        null, collectHeaders, null, timeout,
                        request => _identityServiceHttpRequestConfigurator.ConfigAsync(request, collectIdentityServiceClientInfo, cancellationToken),
                        cancellationToken: cancellationToken);

            System.Console.WriteLine(content);
        }

        public async Task<IEnumerable<string>> GetDataCollectionFileNamesAsync(string dataCollectionName, CancellationToken cancellationToken)
        {
            await Task.CompletedTask;
            throw new NotImplementedException();
        }

        public async Task GarbageCollectDataCollectionFileAsync(string dataCollectionName, string fileName, CancellationToken cancellationToken)
        {
            await Task.CompletedTask;
            throw new NotImplementedException();
        }

        public bool CanGarbageCollect()
        {
            return false;
        }

        public Task<bool> AcceptsFileAsync(string dataCollectionName, string name, long? size, string md5, CancellationToken cancellationToken)
        {
            return Task.FromResult(true);
        }
    }
}
