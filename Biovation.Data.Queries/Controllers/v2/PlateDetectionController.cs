using Biovation.Domain;
using Biovation.Repository.SQL.v2;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Threading.Tasks;

namespace Biovation.Data.Queries.Controllers.v2
{

    [Route("biovation/api/v{version:apiVersion}/[controller]")]
    //[ApiVersion("2.0")]
    public class PlateDetectionController : Controller
    {
        private readonly PlateDetectionRepository _plateDetectionRepository;


        public PlateDetectionController(PlateDetectionRepository plateDetectionRepository)
        {
            _plateDetectionRepository = plateDetectionRepository;
        }

        [HttpGet]
        public Task<ResultViewModel<PagingResult<PlateDetectionLog>>> GetPlateDetectionLog(int logId = default, string licensePlate = default, int detectorId = default, DateTime fromDate = default, DateTime toDate = default, int minPrecision = 0, int maxPrecision = 0, bool withPic = true, bool successTransfer = false, int pageNumber = default,
        int pageSize = default)
        {
            return Task.Run(() => _plateDetectionRepository.GetPlateDetectionLog(logId, licensePlate, detectorId, fromDate, toDate, minPrecision, maxPrecision, withPic, successTransfer, pageNumber, pageSize));
        }

        [HttpGet]
        public Task<ResultViewModel<LicensePlate>> GetLicensePlate(string licensePlate, int entityId)
        {
            return Task.Run(() => _plateDetectionRepository.GetLicensePlate(licensePlate, entityId));
        }

    }
}