using Biovation.Domain;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
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
        [Authorize]

        public Task<ResultViewModel<LicensePlate>> GetLicensePlate(string licensePlate, int entityId)
        {
            return Task.Run(() => _plateDetectionRepository.GetLicensePlate(licensePlate, entityId));
        }

        [HttpGet]
        [Route("PlateDetectionLog")]
        [Authorize]

        public Task<ResultViewModel<PagingResult<PlateDetectionLog>>> GetPlateDetectionLog(int logId = default, string licensePlate = default, int detectorId = default, DateTime fromDate = default, DateTime toDate = default, int minPrecision = 0, int maxPrecision = 0, bool withPic = true, bool successTransfer = false, int pageNumber = default,
       int pageSize = default, string whereClause = "", string orderByClause = "")
        {
            return Task.Run(() => _plateDetectionRepository.GetPlateDetectionLog(logId, licensePlate, detectorId, fromDate, toDate, minPrecision, maxPrecision, withPic, successTransfer, pageNumber, pageSize, whereClause, orderByClause));
        }

        [HttpGet]
        [Route("ReadLicensePlate")]
        [Authorize]

        public Task<ResultViewModel<List<LicensePlate>>> ReadLicensePlate(string licensePlateNumber, int entityId, bool isActive, DateTime startDate, DateTime endDate, TimeSpan intervalBeginningStartTime, TimeSpan intervalEndStartTime, TimeSpan intervalBeginningFinishTime, TimeSpan intervalEndFinishTime)
        {
            return Task.Run(() => _plateDetectionRepository.ReadLicensePlate(licensePlateNumber, entityId, isActive, startDate, endDate, intervalBeginningStartTime, intervalEndStartTime, intervalBeginningFinishTime, intervalEndFinishTime));
        }
    }
}