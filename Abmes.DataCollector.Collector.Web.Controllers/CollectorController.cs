using Abmes.DataCollector.Collector.Services.Contracts;
using Abmes.DataCollector.Collector.Services.Contracts.AppConfig;
using Microsoft.AspNetCore.Mvc;

namespace Abmes.DataCollector.Collector.Web.Controllers;

[Route("[controller]")]
public class CollectorController(
    IMainCollector mainCollector,
    IBootstrapper bootstrapper) : ControllerBase
{
    // POST Collector/collect/configSetName?dataCollections=name1,name2,name3&mode=collect|check
    [Route("collect/{configSetName}")]
    [HttpPost]
    public async Task CollectAsync(string? configSetName, [FromQuery] string? dataCollections, [FromQuery] string? collectorMode, [FromQuery] string? timeFilter, CancellationToken cancellationToken)
    {
        bootstrapper.SetConfig(configSetName, dataCollections, collectorMode, timeFilter);
        await mainCollector.CollectAsync(cancellationToken);
    }
}
