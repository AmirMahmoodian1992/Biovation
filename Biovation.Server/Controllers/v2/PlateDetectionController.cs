using System;
using System.Threading.Tasks;
using Biovation.Domain;
using Biovation.Server.Attribute;
using Biovation.Service.Api.v2;
using Microsoft.AspNetCore.Mvc;

namespace Biovation.Server.Controllers.v2
{
    [Authorize]
    [ApiController]
    [ApiVersion("2.0")]
    [Route("biovation/api/v{version:apiVersion}/[controller]")]
    public class PlateDetectionController : ControllerBase
    {
        private readonly PlateDetectionService _plateDetectionService;

        public PlateDetectionController(PlateDetectionService plateDetectionService)
        {
            _plateDetectionService = plateDetectionService;
        }

        [HttpPost]
        [Authorize]
        public async Task<ResultViewModel> AddLicensePlate([FromBody]LicensePlate licensePlate)
        {
            return await _plateDetectionService.AddLicensePlate(licensePlate, HttpContext.Items["Token"] as string);
        }

        [HttpPost]
        [Authorize]
        [Route("PlateDetectionLog")]
        public async Task<ResultViewModel> AddPlateDetectionLog([FromBody]PlateDetectionLog log)
        {
            return await _plateDetectionService.AddPlateDetectionLog(log, HttpContext.Items["Token"] as string);
        }

        [HttpGet]
        [Authorize]
        [Route("PlateDetectionLog")]
        public async Task<ResultViewModel<PagingResult<PlateDetectionLog>>> GetPlateDetectionLog(int logId = default,
            string licensePlate = default, int detectorId = default, DateTime fromDate = default,
            DateTime toDate = default,
            int minPrecision = 0, int maxPrecision = 0, bool withPic = true, bool successTransfer = false,
            int pageNumber = default,
            int pageSize = default)
        {
            var token = HttpContext.Items["Token"] as string;
            return await _plateDetectionService.GetPlateDetectionLog(logId, licensePlate, detectorId, fromDate, toDate,
                minPrecision, maxPrecision, withPic, successTransfer, pageNumber, pageSize, token);
        }

        [HttpGet]
        [Authorize]
        public async Task<ResultViewModel<LicensePlate>> GetLicensePlate(string licensePlate, int entityId)
        {
            var token = HttpContext.Items["Token"] as string;
            return await _plateDetectionService.GetLicensePlate(licensePlate, entityId, token);
        }

    }
}