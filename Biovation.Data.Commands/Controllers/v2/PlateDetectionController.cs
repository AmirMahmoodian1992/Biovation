using Biovation.Domain;
using Biovation.Repository.SQL.v2;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Threading.Tasks;

namespace Biovation.Data.Queries.Controllers.v2
{

    [Route("biovation/api/queries/v2/[controller]")]
    //[ApiVersion("2.0")]
    public class PlateDetectionController : Controller
    {

        private readonly PlateDetectionRepository _plateDetectionRepository;

        public PlateDetectionController(PlateDetectionRepository PlateDetectionRepository)
        {
            _plateDetectionRepository = PlateDetectionRepository;
        }


        [HttpPost]
        [Route("AddLicensePlate")]
        public Task<ResultViewModel> AddLicensePlate(LicensePlate licensePlate = default)
        {
            return Task.Run(() => _plateDetectionRepository.AddLicensePlate(licensePlate));
        }

        [HttpPost]
        [Route("AddPlateDetectionLog")]
        public Task<ResultViewModel> AddPlateDetectionLog(PlateDetectionLog log)
        {
            return Task.Run(() => _plateDetectionRepository.AddPlateDetectionLog(log));
        }


    }
}