using System.Threading.Tasks;
using Biovation.Domain;
using Biovation.Service.Api.v1;
using Microsoft.AspNetCore.Mvc;

namespace Biovation.Server.Controllers.v2
{
    [Authorize]
    [ApiVersion("2.0")]
    [Route("biovation/api/v2/[controller]")]
    //[Route("biovation/api/v{version:apiVersion}/[controller]")]
    public class PlateDetectionController : Controller
    {
        private readonly PlateDetectionService _plateDetectionService;
        //private readonly RestClient _restClient;

        public PlateDetectionController(PlateDetectionService plateDetectionService)
        {
            _plateDetectionService = plateDetectionService;
        }

        [HttpPost]
        public Task<ResultViewModel> AddLicensePlate(LicensePlate licensePlate = default)
        {
            return Task.Run(() => _plateDetectionService.AddLicensePlate(licensePlate));
        }

    }
}