using Biovation.CommonClasses;
using Biovation.CommonClasses.Manager;
using Biovation.Domain;
using Biovation.Repository.Api.v2;
using Kasra.MessageBus.Domain.Enumerators;
using Kasra.MessageBus.Domain.Interfaces;
using Kasra.MessageBus.Infrastructure;
using Kasra.MessageBus.Managers.Sinks.EventBus;
using Kasra.MessageBus.Managers.Sinks.Internal;
using RestSharp;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Threading.Tasks;


namespace Biovation.Service.Api.v1
{
    public class LogService
    {
        private readonly LogRepository _logRepository;
        private readonly RestClient _logExternalSubmissionRestClient;
        private readonly ISource<DataChangeMessage<Log>> _biovationInternalSource;
        private const string BiovationTopicName = "BiovationLogUpdateEvent";

        public LogService(LogRepository logRepository, BiovationConfigurationManager configurationManager)
        {
            _logRepository = logRepository;
            _logExternalSubmissionRestClient = (RestClient)new RestClient(configurationManager.LogMonitoringApiUrl).UseSerializer(() => new RestRequestJsonSerializer());

            var kafkaServerAddress = configurationManager.KafkaServerAddress;
            _biovationInternalSource = InternalSourceBuilder.Start().SetPriorityLevel(PriorityLevel.Medium)
               .Build<DataChangeMessage<Log>>();

            var biovationKafkaTarget = KafkaTargetBuilder.Start().SetBootstrapServer(kafkaServerAddress).SetTopicName(BiovationTopicName)
                .BuildTarget<DataChangeMessage<Log>>();

            var biovationTaskConnectorNode = new ConnectorNode<DataChangeMessage<Log>>(_biovationInternalSource, biovationKafkaTarget);
            biovationTaskConnectorNode.StartProcess();

        }

        public Task<List<Log>> Logs(int id = default, int deviceId = default, int userId = default, DateTime? fromDate = null,
            DateTime? toDate = null, int pageNumber = default, int pageSize = default, bool successTransfer = default)
        {
            return Task.Run(() => _logRepository.Logs(id, deviceId, userId, fromDate, toDate, pageNumber, pageSize, successTransfer: successTransfer)?.Data?.Data ?? new List<Log>());
        }

        public Task<List<Log>> Logs(DeviceTraffic dTraffic)
        {
            return Task.Run(() => _logRepository.Logs(dTraffic.Id, (int)dTraffic.DeviceId, dTraffic.UserId, dTraffic.FromDate, dTraffic.ToDate, dTraffic.PageNumber, dTraffic.PageSize, dTraffic.Where, dTraffic.Order, dTraffic.OnlineUserId, dTraffic.State)?.Data?.Data ?? new List<Log>());
        }

        public Task<List<Log>> SelectSearchedOfflineLogs(DeviceTraffic dTraffic)
        {
            return Task.Run(() => _logRepository.Logs(dTraffic.Id, (int)dTraffic.DeviceId, dTraffic.UserId, dTraffic.FromDate, dTraffic.ToDate, dTraffic.PageNumber, dTraffic.PageSize, dTraffic.Where, dTraffic.Order, dTraffic.OnlineUserId, dTraffic.State)?.Data?.Data ?? new List<Log>());
        }

        public Task<List<Log>> SelectSearchedOfflineLogsWithPaging(DeviceTraffic dTraffic)
        {
            return Task.Run(() => _logRepository.Logs(dTraffic.Id, (int)dTraffic.DeviceId, dTraffic.UserId, dTraffic.FromDate, dTraffic.ToDate, dTraffic.PageNumber, dTraffic.PageSize, dTraffic.Where, dTraffic.Order, dTraffic.OnlineUserId, dTraffic.State)?.Data?.Data ?? new List<Log>());
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
                    return await _logRepository.AddLog(logs);
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

                            var result = await _logExternalSubmissionRestClient.ExecuteAsync<ResultViewModel>(restRequest);



                            //integration


                            var biovationBrokerMessageData = new List<DataChangeMessage<Log>>

                                  {
                                 new DataChangeMessage<Log>
                                  {
                                      Id = Guid.NewGuid().ToString(), EventId = 1, SourceName = "BiovationCore",
                                      TimeStamp = DateTimeOffset.Now, SourceDatabaseName = "biovation", Data = shortenedLogList

                                      }
                                  };

                            _biovationInternalSource.PushData(biovationBrokerMessageData);



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

        public Task<ResultViewModel> UpdateLog(List<Log> logs)
        {
            return Task.Run(() => _logRepository.UpdateLog(logs));
        }

        public Task<ResultViewModel> AddLogImage(Log log)
        {
            return Task.Run(() => _logRepository.AddLogImage(log));
        }

        public Task<List<Log>> CheckLogInsertion(List<Log> logs)
        {
            return Task.Run(() => _logRepository.CheckLogInsertion(logs));
        }

        public Task<byte[]> GetImage(long id)
        {
            return Task.Run(() =>
           {
               var log = Logs(((int)id)).Result.FirstOrDefault();
               if (log == null || string.IsNullOrEmpty(log.Image)) return new byte[0];
               var path = log.Image;
               var bytes = File.ReadAllBytes(path);
               return bytes;
           });
        }


        public Task<bool> SaveLogsInFile(List<Log> logs, string brandName, uint deviceCode)
        {
            return Task.Run(() =>
            {
                try
                {
                    var path = $"AttLog\\{brandName}\\{DateTime.Now:yyyy-MM-dd-HH-mm-ss}({deviceCode})";
                    if (!Directory.Exists(path))
                    {
                        Directory.CreateDirectory(path);
                    }

                    File.WriteAllLines($"{path}\\Log.Txt", logs.Select(s => s.ToString()));
                    return true;
                }
                catch (Exception exception)
                {
                    Logger.Log(exception);
                    return false;
                }
            });
        }
    }
}
