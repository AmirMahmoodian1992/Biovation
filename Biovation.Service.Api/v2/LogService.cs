using System;
using System.Collections.Generic;
using System.Data;
using System.Threading.Tasks;
using Biovation.Domain;
using Biovation.Repository.Api.v2;

namespace Biovation.Service.Api.v2
{
    public class LogService
    {
        private readonly LogRepository _logRepository;

        public LogService(LogRepository logRepository)
        {
            _logRepository = logRepository;
        }

        public ResultViewModel<PagingResult<Log>> Logs(int id = default, int deviceId = default,
            int userId = default, bool successTransfer = default, DateTime? fromDate = null, DateTime? toDate = null, int pageNumber = default,
            int pageSize = default)
        {
            return _logRepository.Logs(id, deviceId, userId, fromDate, toDate, pageNumber, pageSize, successTransfer:successTransfer);
        }

        public Task<ResultViewModel> AddLog(Log log)
        {
            return Task.Run(() => _logRepository.AddLog(log));
        }
        public Task<ResultViewModel> AddLog(List<Log> logs)
        {
            return Task.Run(() => _logRepository.AddLog(logs));
        }

        public Task<ResultViewModel> UpdateLog(List<Log> logs)
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
