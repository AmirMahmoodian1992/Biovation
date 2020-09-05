using System.Threading.Tasks;
using Biovation.Domain;
using Biovation.Repository.SQL.v2;
using Microsoft.AspNetCore.Mvc;

namespace Biovation.Data.Commands.Controllers.v2
{

    [Route("biovation/api/v2/[controller]")]
    //[ApiVersion("2.0")]
    public class PlateDetectionController : Controller
    {

        private readonly PlateDetectionRepository _plateDetectionRepository;

        public PlateDetectionController(PlateDetectionRepository plateDetectionRepository)
        {
            _plateDetectionRepository = plateDetectionRepository;
        }


        [HttpPost]
        [Route("LicensePlate")]
        public Task<ResultViewModel> AddLicensePlate([FromBody]LicensePlate licensePlate = default)
        {
            return Task.Run(() => _plateDetectionRepository.AddLicensePlate(licensePlate));
        }

        [HttpPost]
        [Route("PlateDetectionLog")]
        public Task<ResultViewModel> AddPlateDetectionLog([FromBody]PlateDetectionLog log)
        {
            return Task.Run(() => _plateDetectionRepository.AddPlateDetectionLog(log));
        }


    }
}