using System.Collections.Generic;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System.Threading.Tasks;
using Biovation.Domain.DashboardModels;

namespace Biovation.Dashboard.Controllers
{
    [Route("biovation/api/[controller]")]
    [ApiController]
    public class MetricsController : ControllerBase
    {
        private readonly ILogger<MetricsController> _logger;


        public MetricsController(ILogger<MetricsController> logger)
        {
            _logger = logger;
        }

        [HttpPost]
        public Task SubmitMetric([FromBody] object metricsData)
        {
            _logger.LogDebug(metricsData.ToString());
            var deserialized = JsonConvert.DeserializeObject<Context>(metricsData.ToString());
            return Task.CompletedTask;
        }
    }
}
