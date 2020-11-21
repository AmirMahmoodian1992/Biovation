using Biovation.CommonClasses;
using Biovation.CommonClasses.Manager;
using Biovation.Domain;
using Biovation.Repository.Api.v2;
using Biovation.Repository.MessageBus;
using RestSharp;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Threading.Tasks;

namespace Biovation.Service.Api.v2
{
    public class LogService
    {
        private readonly RestClient _restClient;
        private readonly LogRepository _logRepository;
        private readonly LogMessageBusRepository _logMessageBusRepository;

        public LogService(LogRepository logRepository, BiovationConfigurationManager configurationManager, LogMessageBusRepository logMessageBusRepository)
        {
            _logRepository = logRepository;
            _logMessageBusRepository = logMessageBusRepository;
            _restClient = (RestClient)new RestClient(configurationManager.LogMonitoringApiUrl).UseSerializer(() => new RestRequestJsonSerializer());
        }

        public Task<ResultViewModel<PagingResult<Log>>> Logs(int id = default, int deviceId = default,
            int userId = default, bool? successTransfer = null, DateTime? fromDate = null, DateTime? toDate = null, int pageNumber = default,
            int pageSize = default, string where = default, string order = default, string token = default)
        {
            return Task.Run(() => _logRepository.Logs(id, deviceId, userId, fromDate, toDate, pageNumber, pageSize, where, order, successTransfer, token));
        }

        public Task<ResultViewModel> AddLog(Log log, string token = default)
        {
            return Task.Run(() => _logRepository.AddLog(log));
        }
        public Task<ResultViewModel> AddLog(List<Log> logs, string token = default)
        {
            return Task.Run(() => _logRepository.AddLog(logs, token));
        }

        public Task<ResultViewModel> UpdateLog(List<Log> logs, string token = default)
        {
            return Task.Run(() => _logRepository.UpdateLog(logs, token));
        }

        public Task<ResultViewModel> AddLogImage(Log log, string token = default)
        {
            return Task.Run(() => _logRepository.AddLogImage(log, token));
        }

        public Task<ResultViewModel> UpdateLog(Log log, string token = default)
        {
            return Task.Run(() => _logRepository.UpdateLog(log, token));
        }

        public Task<List<Log>> CheckLogInsertion(List<Log> logs, string token = default)
        {
            return Task.Run(() => _logRepository.CheckLogInsertion(logs, token));
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

        public Task<List<Log>> SelectSearchedOfflineLogs(DeviceTraffic logFilter, string token = default)
        {
            return Task.Run(() => _logRepository.Logs(logFilter.Id, (int)logFilter.DeviceId, logFilter.UserId, logFilter.FromDate, logFilter.ToDate, logFilter.PageNumber, logFilter.PageSize, logFilter.Where, logFilter.Order, logFilter.State, token)?.Data?.Data ?? new List<Log>());
        }

        public Task<List<Log>> SelectSearchedOfflineLogsWithPaging(DeviceTraffic logFilter, string token = default)
        {
            return Task.Run(() => _logRepository.Logs(logFilter.Id, (int)logFilter.DeviceId, logFilter.UserId, logFilter.FromDate, logFilter.ToDate, logFilter.PageNumber, logFilter.PageSize, logFilter.Where, logFilter.Order, logFilter.State, token)?.Data?.Data ?? new List<Log>());
        }
        public Task<ResultViewModel> TransferLogBulk(List<Log> logs, string token = default)
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
                            restRequest.AddHeader("Authorization", token);


                            var result = await _restClient.ExecuteAsync<ResultViewModel>(restRequest);



                            //integration
                            await _logMessageBusRepository.SendLog(logs.Where(log => !log.SuccessTransfer).ToList());


                            //var biovationBrokerMessageData = new List<DataChangeMessage<Log>>
                            //      {
                            //     new DataChangeMessage<Log>
                            //      {
                            //          Id = Guid.NewGuid().ToString(), EventId = 1, SourceName = "BiovationCore",
                            //          TimeStamp = DateTimeOffset.Now, SourceDatabaseName = "biovation", Data = shortenedLogList
                            //          }
                            //      };

                            //_biovationInternalSource.PushData(biovationBrokerMessageData);



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

    }
}
