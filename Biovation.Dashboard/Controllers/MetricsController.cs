using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

namespace Biovation.Dashboard.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class MetricsController : Controller
    {
        private readonly ILogger<MetricsController> _logger;

        public MetricsController(ILogger<MetricsController> logger)
        {
            _logger = logger;
        }

        [HttpPost]
        public Task SubmitMetric([FromBody] string metricsData)
        {
            Console.WriteLine(metricsData);

            return Task.CompletedTask;
        }
    }
}
