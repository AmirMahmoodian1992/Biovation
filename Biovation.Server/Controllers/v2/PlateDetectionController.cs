using System.Threading.Tasks;
using Biovation.Domain;
using Biovation.Service;
using Microsoft.AspNetCore.Mvc;

namespace Biovation.Server.Controllers.v2
{

    [Route("biovation/api/v{version:apiVersion}/[controller]")]
    [ApiVersion("2.0")]
    public class PlateDetectionController : Controller
    {
        private readonly PlateDetectionService _plateDetectionService;
        //private readonly RestClient _restClient;

        public PlateDetectionController(PlateDetectionService plateDetectionService)
        {
            _plateDetectionService = plateDetectionService;
            //_restClient = (RestClient)new RestClient($"http://localhost:{BiovationConfigurationManager.BiovationWebServerPort}/Biovation/Api/").UseSerializer(() => new RestRequestJsonSerializer());
        }

        [HttpPost]
        public Task<IActionResult> AddLicensePlate(LicensePlate licensePlate = default)
        {
            throw null;
        }

    }
}