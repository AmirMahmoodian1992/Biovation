using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System.Threading.Tasks;

namespace Biovation.Dashboard.Controllers
{
    [Route("biovation/api/[controller]")]
    [ApiController]
    public class MetricsController : Controller
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
            var deserialized = JsonConvert.DeserializeObject<JObject>(metricsData.ToString());
            return Task.CompletedTask;
        }
    }
}
