using Biovation.Domain;
using Biovation.Service.Api.v1;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;

namespace Biovation.Server.Controllers.v1
{

    [Route("biovation/api/v{version:apiVersion}/[controller]")]
    [ApiVersion("1.0")]
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
        [Route("AddLicensePlate")]
        public Task<ResultViewModel> AddLicensePlate([FromBody] LicensePlate licensePlate)
        {
            return Task.Run(() => _plateDetectionService.AddLicensePlate(licensePlate));
        }

    }
}
