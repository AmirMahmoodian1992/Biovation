using Biovation.Domain;
using Microsoft.AspNetCore.Mvc;
using System;
using Biovation.CommonClasses.Extension;
using Biovation.Repository.Sql.v2;

namespace Biovation.Data.Queries.Controllers.v2
{
    [Route("biovation/api/v2/[controller]")]

    public class LogController : Controller
    {

        private readonly LogRepository _logRepository;
        private readonly User _user;


        public LogController(LogRepository logRepository)
        {
            _logRepository = logRepository;
            _user = HttpContext.GetUser();
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

        public ResultViewModel<PagingResult<Log>> Logs(int id = default, int deviceId = default, int userId = default, DateTime? fromDate = null, DateTime? toDate = null, int pageNumber = default, int pageSize = default, string where = "", string order = "", bool? successTransfer = default)
        {

            var logResult = _logRepository.Logs(id, deviceId, userId, fromDate, toDate, pageNumber, pageSize, where, order, _user.Id, successTransfer).Result;
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