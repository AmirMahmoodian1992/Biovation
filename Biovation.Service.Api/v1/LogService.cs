using System;
using Biovation.Domain;
using Biovation.Repository.Api.v2;

namespace Biovation.Service.Api.v1
{
    public class LogService
    {
        private readonly LogRepository _logRepository;

        public LogService(LogRepository logRepository)
        {
            _logRepository = logRepository;
        }

        public ResultViewModel<PagingResult<Domain.Log>> Logs(int id = default, int deviceId = default,
            int userId = default, DateTime? fromDate = null, DateTime? toDate = null, int pageNumber = default,
            int pageSize = default)
        {
            return _logRepository.Logs(id, deviceId, userId, fromDate, toDate, pageNumber, pageSize);
        }


    }
}
