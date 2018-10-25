using Abmes.DataCollector.Collector.Configuration;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace Abmes.DataCollector.ConsoleApp
{
    public class DataFilterProvider : IDataFilterProvider
    {
        public async Task<string> GetDataFilterAsync(CancellationToken cancellationToken)
        {
            var args = Environment.GetCommandLineArgs();

            var result = (args.Length > 2) ? args[2] : null;

            return await Task.FromResult(result);
        }
    }
}
