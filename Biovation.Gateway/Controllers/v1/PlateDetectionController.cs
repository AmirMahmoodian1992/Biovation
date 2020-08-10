using System.Threading.Tasks;
using Biovation.CommonClasses.Manager;
using Biovation.CommonClasses.Models;
using Biovation.CommonClasses.Service;
using Microsoft.AspNetCore.Mvc;
using RestSharp;

namespace Biovation.Gateway.Controllers.v1
{

    [Route("[controller]")]
    [ApiController]
    public class PlateDetectionController : ControllerBase
    {
        private readonly PlateDetectionService _plateDetectionService = new PlateDetectionService();
        private readonly RestClient _restClient;

        public PlateDetectionController()
        {
            _restClient = (RestClient)new RestClient($"http://localhost:{ConfigurationManager.BiovationWebServerPort}/Biovation/Api/").UseSerializer(() => new RestRequestJsonSerializer());
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