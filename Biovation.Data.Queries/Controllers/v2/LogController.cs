using System;
using System.Threading.Tasks;
using Biovation.Domain;
using Biovation.Repository.SQL.v2;
using Microsoft.AspNetCore.Mvc;
using Log = Serilog.Log;


namespace Biovation.Data.Queries.Controllers.v2
{
    [Route("biovation/api/v{version:apiVersion}/[controller]")]
    //[ApiVersion("2.0")]
    public class LogController : Controller
    {

        private readonly LogRepository _logRepository;


        public LogController(LogRepository logRepository)
        {
            _logRepository = logRepository;
        }
        //we should consider the without parameter input version of log
        // and handle searchOfflineLogs with paging or not with  [FromBody]DeviceTraffic dTraffic
        [HttpGet]
        public Task<ResultViewModel<PagingResult<Domain.Log>>> Logs(int id = default,int deviceId = default, int userId = default, DateTime? fromDate = null, DateTime? toDate = null, int pageNumber = default, int pageSize = default)
        {
            return Task.Run(() => _logRepository.Logs(id,deviceId,userId,fromDate,toDate, pageNumber, pageSize));
        }
    }
}