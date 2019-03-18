using Abmes.DataCollector.Collector.Common.Configuration;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Abmes.DataCollector.Collector.ConsoleApp.Initialization
{
    public interface IMainService
    {
        Task<int> MainAsync(CancellationToken cancellationToken, Action<IBootstrapper> bootstrap = null);
    }
}
