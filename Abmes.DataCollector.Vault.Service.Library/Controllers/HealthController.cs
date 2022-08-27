using Microsoft.AspNetCore.Mvc;

namespace Abmes.DataCollector.Vault.Service.Controllers;

[Route("/Health")]
public class HealthController : Controller
{
    [HttpGet]
    public string Get()
    {
        return "OK";
    }
}
