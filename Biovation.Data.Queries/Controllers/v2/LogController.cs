using Biovation.Domain;
using Microsoft.AspNetCore.Mvc;
using System;
using Biovation.Repository.Sql.v2;

namespace Biovation.Data.Queries.Controllers.v2
{
    [Route("biovation/api/v2/[controller]")]

    public class LogController : Controller
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
        public ResultViewModel<PagingResult<Log>> Logs(int id = default, int deviceId = default, int userId = default, DateTime? fromDate = null, DateTime? toDate = null, int pageNumber = default, int pageSize = default, string where = "", string order = "", long onlineUserId = default, bool? successTransfer = default)
        {

            var logResult = _logRepository.Logs(id, deviceId, userId, fromDate, toDate, pageNumber, pageSize, where, order, onlineUserId, successTransfer).Result;
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