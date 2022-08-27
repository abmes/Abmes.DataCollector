using Microsoft.AspNetCore.Mvc;

namespace Abmes.DataCollector.Collector.Service.Controllers;

[Route("[controller]")]
public class HealthController : Controller
{
    [HttpGet]
    public string Get()
    {
        return "OK";
    }
}
