using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
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

        public Task<ResultViewModel<PagingResult<Log>>> Logs(int id = default, int deviceId = default,
            int userId = default, bool successTransfer = default, DateTime? fromDate = null, DateTime? toDate = null, int pageNumber = default,
            int pageSize = default, string token = default)
        {
         
            return Task.Run(() => _logRepository.Logs(id, deviceId, userId, fromDate, toDate, pageNumber, pageSize, successTransfer: successTransfer));
            
        }

        public Task<ResultViewModel> AddLog(Log log, string token = default)
        {
            return Task.Run(() => _logRepository.AddLog(log));
        }
        public Task<ResultViewModel> AddLog(List<Log> logs, string token = default)
        {
            return Task.Run(() => _logRepository.AddLog(logs,token));
        }

        public Task<ResultViewModel> UpdateLog(List<Log> logs, string token = default)
        {
            return Task.Run(() => _logRepository.UpdateLog(logs,token));
        }

        public Task<ResultViewModel> AddLogImage(Log log, string token = default)
        {
            return Task.Run(() => _logRepository.AddLogImage(log,token));
        }

        public Task<ResultViewModel> UpdateLog(Log log, string token = default)
        {
            return Task.Run(() => _logRepository.UpdateLog(log,token));
        }

        public Task<List<Log>> CheckLogInsertion(List<Log> logs, string token = default)
        {
            return Task.Run(() => _logRepository.CheckLogInsertion(logs,token));
        }

        public Task<byte[]> GetImage(long id)
        {
            return Task.Run(() =>
            {
                var log = Logs(((int)id)).Result.Data.Data.FirstOrDefault();
                if (log == null || string.IsNullOrEmpty(log.Image)) return new byte[0];
                var path = log.Image;
                var bytes = File.ReadAllBytes(path);
                return bytes;
            });
        }

    }
}
