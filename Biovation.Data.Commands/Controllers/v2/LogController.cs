using Biovation.Domain;
using Biovation.Repository.SQL.v2;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using Biovation.Constants;
using Newtonsoft.Json;

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
        public Task<ResultViewModel> AddLog([FromBody]List<Log> logs)
        {
            return Task.Run(() =>
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
                return _logRepository.AddLog(logsDataTable);
            });
        }

        [HttpPut]
        [Route("UpdateLog")]
        public Task<ResultViewModel> UpdateLog([FromBody]List<Log> logs)
        {
            return Task.Run(() =>
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
                return _logRepository.UpdateLog(logsDataTable);
            });
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