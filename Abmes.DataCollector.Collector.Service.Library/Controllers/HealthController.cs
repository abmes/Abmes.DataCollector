using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace Abmes.DataCollector.Collector.Service.Controllers
{
    [Route("[controller]")]
    public class HealthController : Controller
    {
        [HttpGet]
        public string Get()
        {
            return "OK";
        }
    }
}
