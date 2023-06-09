﻿using Biovation.Domain;
using Biovation.Server.Attribute;
using Biovation.Service.Api.v2;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Linq;
using System.Threading.Tasks;

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
        public async Task<ResultViewModel> AddLicensePlate([FromBody] LicensePlate licensePlate)
        {
            return await _plateDetectionService.AddLicensePlate(licensePlate, HttpContext.Items["Token"] as string);
        }

        [HttpPost]
        [Authorize]
        [Route("PlateDetectionLog")]
        public async Task<ResultViewModel> AddPlateDetectionLog([FromBody] PlateDetectionLog log)
        {
            return await _plateDetectionService.AddPlateDetectionLog(log, HttpContext.Items["Token"] as string);
        }

        [HttpPost]
        [Authorize]
        [Route("ManualPlateDetectionLog")]
        public async Task<ResultViewModel> AddManualPlateDetectionLog([FromBody] PlateDetectionLog plateDetectionLogData)
        {
            return await _plateDetectionService.AddManualPlateDetectionLog(plateDetectionLogData, HttpContext.Items["Token"] as string);
        }

        [HttpPut]
        [Authorize]
        [Route("{parentLogId:int}/ManualPlateDetectionLog")]
        public async Task<ResultViewModel> AddManualPlateDetectionLogOfExistLog([FromRoute] int parentLogId, [FromBody] PlateDetectionLog manualLogData)
        {
            var parentLogData = await _plateDetectionService.GetAllPlateDetectionLog(logId: parentLogId);
            if (parentLogData?.Data?.Data?.FirstOrDefault() is null || !parentLogData.Success)
                return new ResultViewModel { Success = false, Code = 400, Message = "Wrong parent log id provided" };

            var plateDetectionLogData = new ManualPlateDetectionLog(manualLogData)
            {
                ParentLog = parentLogData.Data.Data.FirstOrDefault()
            };

            return await _plateDetectionService.AddManualPlateDetectionLog(plateDetectionLogData, HttpContext.Items["Token"] as string);
        }

        [HttpGet]
        [Authorize]
        [Route("PlateDetectionLog")]
        public async Task<ResultViewModel<PagingResult<ManualPlateDetectionLog>>> GetAllPlateDetectionLog(string firstLicensePlatePart = default, string secondLicensePlatePart = default, string thirdLicensePlatePart = default, string fourthLicensePlatePart = default, int logId = default,
            string licensePlate = default, int detectorId = default, DateTime fromDate = default,
            DateTime toDate = default,
            int minPrecision = 0, int maxPrecision = 0, bool withPic = true, bool successTransfer = false,
            int pageNumber = default,
            int pageSize = default)
        {
            var token = HttpContext.Items["Token"] as string;
            return await _plateDetectionService.GetAllPlateDetectionLog(firstLicensePlatePart, secondLicensePlatePart, thirdLicensePlatePart, fourthLicensePlatePart, logId, licensePlate, detectorId, fromDate, toDate,
                minPrecision, maxPrecision, withPic, successTransfer, pageNumber, pageSize, token);
        }

        [HttpGet]
        [Authorize]
        [Route("CameraPlateDetectionLog")]
        public async Task<ResultViewModel<PagingResult<ManualPlateDetectionLog>>> GetCameraPlateDetectionLog(string firstLicensePlatePart = default, string secondLicensePlatePart = default, string thirdLicensePlatePart = default, string fourthLicensePlatePart = default, int logId = default,
            string licensePlate = default, int detectorId = default, DateTime fromDate = default,
            DateTime toDate = default,
            int minPrecision = 0, int maxPrecision = 0, bool withPic = true, bool successTransfer = false,
            int pageNumber = default,
            int pageSize = default)
        {
            var token = HttpContext.Items["Token"] as string;
            return await _plateDetectionService.GetCameraPlateDetectionLog(firstLicensePlatePart, secondLicensePlatePart, thirdLicensePlatePart, fourthLicensePlatePart, logId, licensePlate, detectorId, fromDate, toDate,
                minPrecision, maxPrecision, withPic, successTransfer, pageNumber, pageSize, token);
        }

        [HttpGet]
        [Authorize]
        [Route("ManualPlateDetectionLog")]
        public async Task<ResultViewModel<PagingResult<ManualPlateDetectionLog>>> GetManualPlateDetectionLog(int logId = default, long userId = default, long parentLogId = default, string licensePlate = default, int detectorId = default, DateTime fromDate = default, DateTime toDate = default,
            int minPrecision = 0, int maxPrecision = 0, bool withPic = true, bool successTransfer = false, int pageNumber = default,
            int pageSize = default)
        {
            var token = HttpContext.Items["Token"] as string;
            return await _plateDetectionService.GetManualPlateDetectionLog(logId, userId, parentLogId, licensePlate, detectorId, fromDate, toDate,
                minPrecision, maxPrecision, withPic, successTransfer, pageNumber, pageSize, token);
        }

        [HttpGet]
        [Authorize]
        public async Task<ResultViewModel<LicensePlate>> GetLicensePlate(string licensePlate, int entityId)
        {
            var token = HttpContext.Items["Token"] as string;
            return await _plateDetectionService.GetLicensePlate(licensePlate, entityId, token);
        }

        [HttpGet]
        [Authorize]
        [Route("PreviousPlateDetectionLog")]
        public async Task<ResultViewModel<PlateDetectionLog>> SelectPreviousPlateDetectionLog(int id = default, string licensePlateNumber = default, DateTime? logDateTime = null)
        {
            var token = HttpContext.Items["Token"] as string;
            return await _plateDetectionService.SelectPreviousPlateDetectionLog(id, licensePlateNumber, logDateTime, token);
        }
    }
}