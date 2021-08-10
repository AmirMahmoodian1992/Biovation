using Biovation.Domain;
using Biovation.Repository.Sql.v2;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Threading.Tasks;

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
        [Route("LicensePlate")]
        public Task<ResultViewModel<LicensePlate>> GetLicensePlate(string licensePlate, int entityId)
        {
            return Task.Run(() => _plateDetectionRepository.GetLicensePlate(licensePlate, entityId));
        }

        [HttpGet]
        [Authorize]
        public Task<ResultViewModel<PagingResult<ManualPlateDetectionLog>>> GetAllPlateDetectionLog(string firstLicensePlatePart = default, string secondLicensePlatePart = default, string thirdLicensePlatePart = default, string fourthLicensePlatePart = default, int logId = default, string licensePlate = default, int detectorId = default, DateTime fromDate = default, DateTime toDate = default, int minPrecision = 0, int maxPrecision = 0, bool withPic = true, bool successTransfer = false, int pageNumber = default,
            int pageSize = default)
        {
            return Task.Run(() => _plateDetectionRepository.GetAllPlateDetectionLog(firstLicensePlatePart, secondLicensePlatePart, thirdLicensePlatePart, fourthLicensePlatePart, logId, licensePlate, detectorId, fromDate, toDate, minPrecision, maxPrecision, withPic, successTransfer, pageNumber, pageSize));
        }

        [HttpGet]
        [Authorize]
        [Route("CameraPlateDetectionLog")]
        public Task<ResultViewModel<PagingResult<PlateDetectionLog>>> GetCameraPlateDetectionLog(string firstLicensePlatePart = default, string secondLicensePlatePart = default, string thirdLicensePlatePart = default, string fourthLicensePlatePart = default, int logId = default, string licensePlate = default, int detectorId = default, DateTime fromDate = default, DateTime toDate = default, int minPrecision = 0, int maxPrecision = 0, bool withPic = true, bool successTransfer = false, int pageNumber = default,
            int pageSize = default)
        {
            return Task.Run(() => _plateDetectionRepository.GetCameraPlateDetectionLog(firstLicensePlatePart, secondLicensePlatePart, thirdLicensePlatePart, fourthLicensePlatePart, logId, licensePlate, detectorId, fromDate, toDate, minPrecision, maxPrecision, withPic, successTransfer, pageNumber, pageSize));
        }

        [HttpGet]
        [Authorize]
        [Route("PreviousPlateDetectionLog")]
        public Task<ResultViewModel<PlateDetectionLog>> SelectPreviousPlateDetectionLog(int id = default, string licensePlateNumber = default, DateTime? logDateTime = null)
        {
            return Task.Run(() => _plateDetectionRepository.SelectPreviousPlateDetectionLog(id, licensePlateNumber, logDateTime));
        }

        [HttpGet]
        [Authorize]
        [Route("ManualPlateDetectionLog")]
        public Task<ResultViewModel<PagingResult<ManualPlateDetectionLog>>> GetManualPlateDetectionLog(
            int logId = default, long userId = default, long parentLogId = default, string licensePlate = default,
            int detectorId = default, DateTime fromDate = default, DateTime toDate = default, int minPrecision = 0,
            int maxPrecision = 0, bool withPic = true, bool successTransfer = false, int pageNumber = default,
            int pageSize = default, string whereClause = default, string orderByClause = default)
        {
            return Task.Run(() => _plateDetectionRepository.GetManualPlateDetectionLog(logId, userId, parentLogId, licensePlate, detectorId, fromDate, toDate, minPrecision, maxPrecision, withPic, successTransfer, pageNumber, pageSize, whereClause, orderByClause));
        }
    }
}