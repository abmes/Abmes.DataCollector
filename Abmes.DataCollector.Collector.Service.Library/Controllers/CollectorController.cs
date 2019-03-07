using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Abmes.DataCollector.Collector.Common.Collecting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace Abmes.DataCollector.Collector.Service.Controllers
{
    [Route("[controller]")]
    public class CollectorController : Controller
    {
        private readonly IMainCollector _mainCollector;

        public CollectorController(IMainCollector mainCollector)
        {
            _mainCollector = mainCollector;
        }

        // POST Collector/collect/configSetName
        [Route("collect/{configSetName}")]
        [HttpPost]
        public async Task CollectAsync(string configSetName, CancellationToken cancellationToken)
        {
            await _mainCollector.CollectAsync(configSetName, cancellationToken);
        }
    }
}
