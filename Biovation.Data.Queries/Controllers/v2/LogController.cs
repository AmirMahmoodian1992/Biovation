﻿using Biovation.Domain;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Linq;
using Biovation.CommonClasses.Extension;
using Biovation.Repository.Sql.v2;

namespace Biovation.Data.Queries.Controllers.v2
{
    [ApiController]
    [Route("biovation/api/v2/[controller]")]
    public class LogController : ControllerBase
    {
        private readonly LogRepository _logRepository;
        public LogController(LogRepository logRepository)
        {
            _logRepository = logRepository;
        }

        //we should consider the without parameter input version of log
        // and handle searchOfflineLogs with paging or not with  [FromBody]DeviceTraffic dTraffic
        /*[HttpGet]
        public Task<> Logs(int id = default,int deviceId = default, int userId = default, DateTime? fromDate = null, DateTime? toDate = null, int pageNumber = default, int pageSize = default)
        {
            return Task.Run(() => _logRepository.Logs(id,deviceId,userId,fromDate,toDate, pageNumber, pageSize));
        }*/

        [HttpGet]
        [Authorize]
        public ResultViewModel<PagingResult<Log>> Logs(int id = default, int deviceGroupId = default, int deviceId = default, int userId = default, DateTime? fromDate = null, DateTime? toDate = null, int pageNumber = default, int pageSize = default, string where = "", string order = "", bool? successTransfer = null)
        {

            var logResult = _logRepository.Logs(id, deviceGroupId, deviceId, userId, fromDate, toDate, pageNumber, pageSize, where, order, HttpContext.GetUser().Id, successTransfer).Result;
            var result = new PagingResult<Log>
            {
                Data = logResult,
                PageSize = pageSize,
                PageNumber = pageNumber,
                From = pageNumber * pageSize,
                Count = logResult.FirstOrDefault()?.Total ?? 0
            };

            return new ResultViewModel<PagingResult<Log>>
            {
                Data = result
            };
        }

        [HttpGet]
        [Route("LogImage/{id?}")]
        public ResultViewModel<PagingResult<Log>> LogImage([FromRoute] int id = default)
        {

            var logResult = _logRepository.LogImage(id).Result;
            var result = new PagingResult<Log>
            {
                Data = logResult,
                Count = logResult.Count
            };

            return new ResultViewModel<PagingResult<Log>>
            {
                Data = result
            };
        }
    }
}