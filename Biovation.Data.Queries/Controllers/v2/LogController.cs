using Biovation.Domain;
using Biovation.Repository.SQL.v2;
using Microsoft.AspNetCore.Mvc;

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
        public ResultViewModel<PagingResult<Log>> Logs(DeviceTraffic deviceTraffic)
        {

            var logResult = _logRepository.Logs(deviceTraffic).Result;
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