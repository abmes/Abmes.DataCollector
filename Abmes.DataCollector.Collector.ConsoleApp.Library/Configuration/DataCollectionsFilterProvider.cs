using Abmes.DataCollector.Collector.Common.Configuration;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace Abmes.DataCollector.ConsoleApp
{
    public class DataCollectionsFilterProvider : IDataCollectionsFilterProvider
    {
        public async Task<string> GetDataCollectionsFilterAsync(CancellationToken cancellationToken)
        {
            var args = Environment.GetCommandLineArgs();

            var result = (args.Length > 2) ? args[2] : null;

            return await Task.FromResult(result);
        }
    }
}
