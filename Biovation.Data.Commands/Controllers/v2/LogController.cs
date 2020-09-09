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



        public Task<ResultViewModel> AddLog(Log log)
        {
            return Task.Run(() => _logRepository.AddLog(log));
        }

        public Task<ResultViewModel> AddLog(DataTable logs)
        {
            return Task.Run(() => _logRepository.AddLog(logs));
        }



        public Task<ResultViewModel> UpdateLog(DataTable logs)
        {
            return Task.Run(() => _logRepository.UpdateLog(logs));
        }



        public Task<ResultViewModel> AddLogImage(Log log)
        {
            return Task.Run(() => _logRepository.AddLogImage(log));
        }

        public Task<ResultViewModel> UpdateLog(Log log)
        {
            return Task.Run(() => _logRepository.UpdateLog(log));
        }



        public Task<List<Log>> CheckLogInsertion(List<Log> logs)
        {

            return Task.Run(() => _logRepository.CheckLogInsertion(logs));
        }
    }
}