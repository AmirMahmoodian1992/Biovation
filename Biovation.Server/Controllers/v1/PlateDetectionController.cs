using System.Threading.Tasks;
using Biovation.CommonClasses.Manager;
using Biovation.CommonClasses.Models;
using Biovation.CommonClasses.Service;
using Microsoft.AspNetCore.Mvc;
using RestSharp;

namespace Biovation.Gateway.Controllers.v1
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
        public Task<ResultViewModel> AddLicensePlate(LicensePlate licensePlate)
        {
            return Task.Run(async () =>
            {
                var result = _plateDetectionService.AddLicensePlate(licensePlate).Result;
                if (result.Validate != 1) return result;
                return result;
            });
        }

    }
}