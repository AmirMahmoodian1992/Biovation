using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using Biovation.CommonClasses;
using Biovation.CommonClasses.Manager;
using Biovation.Constants;
using Biovation.Domain;
using Biovation.Repository.Api.v2;
using Newtonsoft.Json;
using RestSharp;

namespace Biovation.Service.Api.v1
{
    public class LogService
    {
        private readonly LogRepository _logRepository;
        private readonly RestClient _logExternalSubmissionRestClient;

        public LogService(LogRepository logRepository)
        {
            _logRepository = logRepository;
            _logExternalSubmissionRestClient = (RestClient)new RestClient(BiovationConfigurationManager.LogMonitoringApiUrl).UseSerializer(() => new RestRequestJsonSerializer());
        }

        public Task<List<Log>> Logs(int id = default, int deviceId = default,
            int userId = default, bool successTransfer = default, DateTime? fromDate = null, DateTime? toDate = null, int pageNumber = default,
            int pageSize = default)
        {
            return Task.Run(() => _logRepository.Logs(id, deviceId, userId, successTransfer, fromDate, toDate, pageNumber, pageSize).Data.Data);
        }

        public Task<ResultViewModel> AddLog(Log log)
        {
            return Task.Run(() => _logRepository.AddLog(log));
        }
        public Task<ResultViewModel> AddLog(List<Log> logs)
        {
            return Task.Run(async () =>
            {
                try
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
                }
                catch (Exception)
                {
                    return new ResultViewModel { Validate = 0 };
                }
            });
        }

        public Task<ResultViewModel> TransferLogBulk(List<Log> logs)
        {
            return Task.Run(async () =>
            {
                try
                {
                    var deviceCodes = logs.GroupBy(g => g.DeviceCode).Select(s => s.Key).ToList();
                    foreach (var deviceCode in deviceCodes)
                    {
                        var deviceLogs = logs.Where(x => x.DeviceCode == deviceCode).OrderByDescending(x => x.LogDateTime).ToList();
                        //برای ارسال لاگ به صورت 1000 تا 1000تا
                        var logsCount = deviceLogs.Count;
                        var loopUpperBound = logsCount / 1000;
                        loopUpperBound = loopUpperBound == 0 ? 1 : loopUpperBound;
                        loopUpperBound = logsCount % 1000 <= 0 ? loopUpperBound : loopUpperBound + 1;

                        for (var i = 0; i < loopUpperBound; i++)
                        {
                            var shortenedLogList = deviceLogs.Skip(i * 1000).Take(1000).ToList();

                            var restRequest = new RestRequest("UpdateAttendance/UpdateAttendanceBulk", Method.POST, DataFormat.Json);
                            restRequest.AddJsonBody(shortenedLogList);

                            var result = await _logExternalSubmissionRestClient.ExecuteTaskAsync<ResultViewModel>(restRequest);
                            //var data = JsonConvert.SerializeObject(shortenedLogList);
                            //var result = _communicationManagerAtt.CallRest(
                            //    "/api/Biovation/UpdateAttendance/UpdateAttendanceBulk", "Post", null, data);

                            if (!result.IsSuccessful || result.StatusCode != HttpStatusCode.OK || result.Data.Validate != 1) continue;

                            //shortenedLogList = shortenedLogList.Select(log => new Log
                            //{
                            //    Id = log.Id,
                            //    DeviceId = log.DeviceId,
                            //    DeviceCode = log.DeviceCode,
                            //    EventId = log.EventId,
                            //    UserId = log.UserId,
                            //    LogDateTime = log.LogDateTime,
                            //    DateTimeTicks = log.DateTimeTicks,
                            //    SubEvent = log.SubEvent,
                            //    TnaEvent = log.TnaEvent,
                            //    MatchingType = log.MatchingType,
                            //    SuccessTransfer = true
                            //});

                            shortenedLogList.ForEach(log => log.SuccessTransfer = true);

                            await UpdateLog(shortenedLogList);
                        }
                    }

                    return new ResultViewModel { Validate = 1 };
                }
                catch (Exception exception)
                {
                    Logger.Log(exception);
                    return new ResultViewModel { Validate = 0 };
                }
            });
        }

        public Task<ResultViewModel> UpdateLog(DataTable logs)
        {
            return Task.Run(() => _logRepository.UpdateLog(logs));
        }

        public Task<ResultViewModel> AddLogImage(Log log)
        {
            return Task.Run(() => _logRepository.AddLogImage(log));
        }

        public Task<ResultViewModel> UpdateLog(IEnumerable<Log> logs)
        {
            return Task.Run(async () =>
            {
                try
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
                }
                catch (Exception exception)
                {
                    Logger.Log(exception);
                    return new ResultViewModel { Validate = 0, Message = exception.ToString() };
                }
            });
        }

        public Task<List<Log>> CheckLogInsertion(List<Log> logs)
        {
            return Task.Run(() => _logRepository.CheckLogInsertion(logs));
        }
    }
}
