using Biovation.Constants;
using Biovation.Domain;
using Biovation.Repository.MessageBus;
using Biovation.Repository.SQL.v2;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Threading.Tasks;

namespace Biovation.Data.Commands.Controllers.v2
{
    [Route("biovation/api/v2/[controller]")]
    public class LogController : Controller
    {
        private readonly LogRepository _logRepository;
        private readonly LogMessageBusRepository _logMessageBusRepository;
        public LogController(LogRepository logRepository, LogMessageBusRepository logMessageBusRepository)
        {
            _logRepository = logRepository;
            _logMessageBusRepository = logMessageBusRepository;
        }

        [HttpPost]
        [Route("AddLog")]
        public Task<ResultViewModel> AddLog([FromBody]Log log)
        {
            var logInsertionResult = _logRepository.AddLog(log);
            if (!logInsertionResult.Result.Success) return logInsertionResult;
            //integration
            var logList = new List<Log> { log };
            _logMessageBusRepository.SendLog(logList);

            return logInsertionResult;
        }

        [HttpPost]
        [Route("AddLogBulk")]
        public Task<ResultViewModel> AddLog([FromBody]List<Log> logs)
        {
            return Task.Run(async () =>
            {
                var json = JsonConvert.SerializeObject(logs.Select(s => new
                {
                    s.Id,
                    s.DeviceId,
                    s.DeviceCode,
                    EventId = s.EventLog.Code,
                    s.UserId,
                    datetime = s.LogDateTime,
                    Ticks = s.DateTimeTicks,
                    SubEvent = s.SubEvent?.Code ?? LogSubEvents.NormalCode,
                    TNAEvent = s.TnaEvent,
                    s.InOutMode,
                    MatchingType = s.MatchingType?.Code,
                    //s.MatchingType,
                    s.SuccessTransfer
                }));

                var logsDataTable = JsonConvert.DeserializeObject<DataTable>(json);
                return await _logRepository.AddLog(logsDataTable);
            });
        }

        [HttpPut]
        [Route("UpdateLog")]
        public Task<ResultViewModel> UpdateLog([FromBody]List<Log> logs)
        {
            return Task.Run(async () =>
            {
                var serializedData = JsonConvert.SerializeObject(logs.Select(s => new
                {
                    s.Id,
                    s.DeviceId,
                    s.DeviceCode,
                    EventId = s.EventLog.Code,
                    s.UserId,
                    datetime = s.LogDateTime,
                    Ticks = s.DateTimeTicks,
                    SubEvent = s.SubEvent?.Code ?? LogSubEvents.NormalCode,
                    TNAEvent = s.TnaEvent,
                    s.InOutMode,
                    s.MatchingType.Code,
                    s.SuccessTransfer
                }));
                var logsDataTable = JsonConvert.DeserializeObject<DataTable>(serializedData);
                return await _logRepository.UpdateLog(logsDataTable);
            });
        }
        [HttpPatch]
        [Route("AddLogImage")]
        public Task<ResultViewModel> AddLogImage([FromBody]Log log)
        {
            return Task.Run(async () => await _logRepository.AddLogImage(log));
        }


        [HttpPut]
        [Route("UpdateLog")]
        public Task<ResultViewModel> UpdateLog([FromBody]Log log)
        {
            return Task.Run(async () => await _logRepository.UpdateLog(log));
        }

        [HttpPut]
        [Route("CheckLogInsertion")]
        public Task<List<Log>> CheckLogInsertion([FromBody]List<Log> logs)
        {
            return Task.Run(async () => await _logRepository.CheckLogInsertion(logs));
        }
    }
}