using Biovation.Domain;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Threading.Tasks;
using Biovation.Repository.Sql.v2;

namespace Biovation.Data.Queries.Controllers.v2
{
    [ApiController]
    [Route("biovation/api/v2/[controller]")]
    //[ApiVersion("2.0")]
    public class PlateDetectionController : ControllerBase
    {
        private readonly PlateDetectionRepository _plateDetectionRepository;

        public PlateDetectionController(PlateDetectionRepository plateDetectionRepository)
        {
            _plateDetectionRepository = plateDetectionRepository;
        }

        [HttpGet]
        [Route("LicensePlate")]
        [Authorize]

        public Task<ResultViewModel<LicensePlate>> GetLicensePlate(string licensePlate, int entityId)
        {
            return Task.Run(() => _plateDetectionRepository.GetLicensePlate(licensePlate, entityId));
        }

        [HttpGet]
        [Authorize]

        public Task<ResultViewModel<PagingResult<PlateDetectionLog>>> GetPlateDetectionLog(string firstLicensePlatePart = default, string secondLicensePlatePart = default, string thirdLicensePlatePart = default, string fourthLicensePlatePart = default, int logId = default, string licensePlate = default, int detectorId = default, DateTime fromDate = default, DateTime toDate = default, int minPrecision = 0, int maxPrecision = 0, bool withPic = true, bool successTransfer = false, int pageNumber = default,
       int pageSize = default)
        {
            return Task.Run(() => _plateDetectionRepository.GetPlateDetectionLog(firstLicensePlatePart, secondLicensePlatePart,thirdLicensePlatePart,fourthLicensePlatePart, logId, licensePlate, detectorId, fromDate, toDate, minPrecision, maxPrecision, withPic, successTransfer, pageNumber, pageSize));
        }


        [HttpGet]
        [Route("ManualPlateDetectionLog")]
        [Authorize]
        public Task<ResultViewModel<PagingResult<ManualPlateDetectionLog>>> GetManualPlateDetectionLog(
            int logId = default, long userId = default, long parentLogId = default, string licensePlate = default,
            int detectorId = default, DateTime fromDate = default, DateTime toDate = default, int minPrecision = 0,
            int maxPrecision = 0, bool withPic = true, bool successTransfer = false, int pageNumber = default,
            int pageSize = default)
        {
            return Task.Run(() => _plateDetectionRepository.GetManualPlateDetectionLog( logId, userId,parentLogId, licensePlate, detectorId, fromDate, toDate, minPrecision, maxPrecision, withPic, successTransfer, pageNumber, pageSize));
        }
    }
}