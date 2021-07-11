using Biovation.CommonClasses;
using Biovation.Domain;
using Biovation.Repository.Api.v2;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace Biovation.Service.Api.v2
{
    public class LogService
    {
        private readonly LogRepository _logRepository;

        public LogService(LogRepository logRepository)
        {
            _logRepository = logRepository;
        }

        public async Task<ResultViewModel<PagingResult<Log>>> Logs(int id = default,int deviceGroupId = default, int deviceId = default,
            int userId = default, bool? successTransfer = null, DateTime? fromDate = null, DateTime? toDate = null, int pageNumber = default,
            int pageSize = default, string where = default, string order = default, string token = default)
        {
            return await _logRepository.Logs(id, deviceGroupId, deviceId, userId, fromDate, toDate, pageNumber, pageSize, where, order, successTransfer, token);
        }

        public async Task<ResultViewModel> AddLog(Log log, string token = default)
        {
            return await _logRepository.AddLog(log, token);
        }
        public async Task<ResultViewModel> AddLog(List<Log> logs, string token = default)
        {
            return await _logRepository.AddLog(logs, token);
        }

        public async Task<ResultViewModel> UpdateLog(List<Log> logs, string token = default)
        {
            return await _logRepository.UpdateLog(logs, token);
        }

        public async Task<ResultViewModel> AddLogImage(Log log, string token = default)
        {
            return await _logRepository.AddLogImage(log, token);
        }

        public async Task<ResultViewModel> UpdateLog(Log log, string token = default)
        {
            return await _logRepository.UpdateLog(log, token);
        }

        public async Task<List<Log>> CheckLogInsertion(List<Log> logs, string token = default)
        {
            return await _logRepository.CheckLogInsertion(logs, token);
        }

        public async Task<byte[]> GetImage(long id)
        {
            var log = (await _logRepository.LogImage(id)).Data.Data.FirstOrDefault();
            if (log == null || string.IsNullOrEmpty(log.Image)) return new byte[0];
            var path = log.Image;
            var bytes = File.ReadAllBytes(path);
            return bytes;
        }

        public async Task<List<Log>> SelectSearchedOfflineLogs(DeviceTraffic logFilter, string token = default)
        {
            return (await _logRepository.Logs(logFilter.Id, logFilter.deviceGroupId, (int)logFilter.DeviceId, logFilter.UserId, logFilter.FromDate, logFilter.ToDate, logFilter.PageNumber, logFilter.PageSize, logFilter.Where, logFilter.Order, logFilter.State, token))?.Data?.Data ?? new List<Log>();
        }

        public async Task<List<Log>> SelectSearchedOfflineLogsWithPaging(DeviceTraffic logFilter, string token = default)
        {
            return (await _logRepository.Logs(logFilter.Id, logFilter.deviceGroupId, (int)logFilter.DeviceId, logFilter.UserId, logFilter.FromDate, logFilter.ToDate, logFilter.PageNumber, logFilter.PageSize, logFilter.Where, logFilter.Order, logFilter.State, token))?.Data?.Data ?? new List<Log>();
        }

        public async Task<ResultViewModel> TransferLogBulk(List<Log> logs, string token = default)
        {
            try
            {
                return await _logRepository.BroadcastLogs(logs, token);
            }
            catch (Exception exception)
            {
                Logger.Log(exception);
                return new ResultViewModel { Validate = 0 };
            }
        }
    }
}
