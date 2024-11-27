using Abmes.DataCollector.Collector.Services.Contracts;
using Microsoft.AspNetCore.Mvc;

namespace Abmes.DataCollector.Collector.Web.Controllers;

[Route("[controller]")]
public class CollectorController(
    IMainService mainService)
    : ControllerBase
{
    // POST Collector/collect/configSetName?dataCollections=name1,name2,name3&mode=collect|check
    [Route("collect/{configSetName}")]
    [HttpPost]
    public async Task CollectAsync(string? configSetName, [FromQuery] string? dataCollections, [FromQuery] string? collectorMode, [FromQuery] string? timeFilter, CancellationToken cancellationToken)
    {
        var collectorParams = new CollectorParams
        {
            ConfigSetName = configSetName,
            DataCollectionNames = dataCollections,
            CollectorMode = collectorMode,
            TimeFilter = timeFilter
        };

        await mainService.MainAsync(collectorParams, 0, cancellationToken);
    }
}
