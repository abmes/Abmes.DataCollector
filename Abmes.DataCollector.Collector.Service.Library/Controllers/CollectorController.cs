using Abmes.DataCollector.Collector.Common.Collecting;
using Abmes.DataCollector.Collector.Common.Configuration;
using Microsoft.AspNetCore.Mvc;

namespace Abmes.DataCollector.Collector.Service.Controllers;

[Route("[controller]")]
public class CollectorController : ControllerBase
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
    public async Task CollectAsync(string? configSetName, [FromQuery] string? dataCollections, [FromQuery] string? collectorMode, [FromQuery] string? timeFilter, CancellationToken cancellationToken)
    {
        _bootstrapper.SetConfig(configSetName, dataCollections, collectorMode, timeFilter);
        await _mainCollector.CollectAsync(cancellationToken);
    }
}
