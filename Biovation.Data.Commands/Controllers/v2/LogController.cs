using Biovation.Domain;
using Biovation.Repository.SQL.v2;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.Data;
using System.Threading.Tasks;

namespace Biovation.Data.Commands.Controllers.v2
{
    [Route("biovation/api/v2/[controller]")]
    public class LogController : Controller
    {

        private readonly LogRepository _logRepository;


        public LogController(LogRepository logRepository)
        {
            _logRepository = logRepository;
        }

        [HttpPost]
        [Route("AddLog")]
        public Task<ResultViewModel> AddLog([FromBody]Log log)
        {
            return Task.Run(() => _logRepository.AddLog(log));
        }

        [HttpPost]
        [Route("AddLogBulk")]
        public Task<ResultViewModel> AddLog([FromBody]DataTable logs)
        {
            return Task.Run(() => _logRepository.AddLog(logs));
        }

        [HttpPut]
        [Route("UpdateLog")]
        public Task<ResultViewModel> UpdateLog([FromBody]DataTable logs)
        {
            return Task.Run(() => _logRepository.UpdateLog(logs));
        }
        [HttpPatch]
        [Route("AddLogImage")]
        public Task<ResultViewModel> AddLogImage([FromBody]Log log)
        {
            return Task.Run(() => _logRepository.AddLogImage(log));
        }

        [HttpPut]
        [Route("UpdateLog")]
        public Task<ResultViewModel> UpdateLog([FromBody]Log log)
        {
            return Task.Run(() => _logRepository.UpdateLog(log));
        }

        [HttpPut]
        [Route("CheckLogInsertion")]
        public Task<List<Log>> CheckLogInsertion([FromBody]List<Log> logs)
        {

            return Task.Run(() => _logRepository.CheckLogInsertion(logs));
        }
    }
}