﻿using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Abmes.DataCollector.Collector.Common.Collecting;
using Abmes.DataCollector.Collector.Common.Configuration;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace Abmes.DataCollector.Collector.Service.Controllers
{
    [Route("[controller]")]
    public class CollectorController : Controller
    {
        private readonly IMainCollector _mainCollector;
        private readonly IBootstrapper _bootstrapper;

        public CollectorController(
            IMainCollector mainCollector,
            IBootstrapper bootstrapper)
        {
            _mainCollector = mainCollector;
            _bootstrapper = bootstrapper;
        }

        // POST Collector/collect/configSetName?dataCollections=name1,name2,name3&mode=collect|check
        [Route("collect/{configSetName}")]
        [HttpPost]
        public async Task CollectAsync(string configSetName, [FromQuery] string dataCollections, [FromQuery] string collectorMode, CancellationToken cancellationToken)
        {
            _bootstrapper.SetConfig(configSetName, dataCollections, collectorMode);
            await _mainCollector.CollectAsync(cancellationToken);
        }
    }
}
