using Biovation.CommonClasses;
using Biovation.Domain;
using Biovation.Repository.Api.v2;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace Biovation.Service.Api.v2
{
    public class LogService
    {
        private readonly LogRepository _logRepository;
        private readonly PlateDetectionRepository _plateDetectionRepository;

        public LogService(LogRepository logRepository, PlateDetectionRepository plateDetectionRepository)
        {
            _logRepository = logRepository;
            _plateDetectionRepository = plateDetectionRepository;
        }

        public Task<ResultViewModel<PagingResult<Log>>> Logs(int id = default, int deviceId = default,
            int userId = default, bool? successTransfer = null, DateTime? fromDate = null, DateTime? toDate = null, int pageNumber = default,
            int pageSize = default, string where = default, string order = default, string token = default)
        {
            return Task.Run(() => _logRepository.Logs(id, deviceId, userId, fromDate, toDate, pageNumber, pageSize, where, order, successTransfer, token));
        }

        public Task<ResultViewModel> AddLog(Log log, string token = default)
        {
            return Task.Run(() => _logRepository.AddLog(log, token));
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
                var log = _logRepository.LogImage(id).Data.Data.FirstOrDefault();
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
            var logList = new List<Log>();
            var plateLogs = new List<PlateDetectionLog>();
            return Task.Run( () =>
            {
                logList.AddRange(_logRepository.Logs(logFilter.Id, (int)logFilter.DeviceId, logFilter.UserId, logFilter.FromDate, logFilter.ToDate, logFilter.PageNumber, logFilter.PageSize, logFilter.Where, logFilter.Order, logFilter.State, token)?.Data?.Data ?? new List<Log>());
                plateLogs.AddRange(_plateDetectionRepository.GetPlateDetectionLog(logId: logFilter.UserId, detectorId: (int)logFilter.DeviceId, fromDate: logFilter.FromDate ?? new DateTime(2000, 1, 1), toDate: logFilter.ToDate ?? new DateTime(2100, 1, 1)
                , pageNumber: logFilter.PageNumber, pageSize: logFilter.PageSize, whereClause: string.IsNullOrWhiteSpace(logFilter.Where) ? "" : logFilter.Where
                , orderByClause: string.IsNullOrWhiteSpace(logFilter.Order) ? "" : logFilter.Order, withPic: false).Data.Data);
                foreach (var plateLog in plateLogs)
                {
                    try
                    {
                        var str = plateLog.LicensePlate.LicensePlateNumber;
                        plateLog.LicensePlate.LicensePlateNumber = str.Substring(3, 3) + "-" + str.Substring(6, 2) +
                                                                   str.Substring(2, 1) + str.Substring(0, 2);
                    }
                    catch (Exception)
                    {
                        // ignored
                    }
                }

                logList.AddRange(plateLogs.Select(plateLog => new Log
                {
                    Id = plateLog.Id,
                    UserId = plateLog.LicensePlate.EntityId,
                    DeviceCode = (uint)plateLog.DetectorId,
                    DeviceName = plateLog.DeviceName,
                    EventLog = plateLog.EventLog,
                    SurName = plateLog.LicensePlate.LicensePlateNumber,
                    LogDateTime = plateLog.LogDateTime,
                    PicByte = plateLog.FullImage,
                    Time = plateLog.LogDateTime.ToString(CultureInfo.InvariantCulture),
                    Total = plateLog.Total,
                    SubEvent = new Lookup
                    {
                        Category = new LookupCategory
                        {
                            Id = 8
                        },
                        Code = "17001"
                    },
                    MatchingType = new Lookup
                    {
                        Category = new LookupCategory
                        {
                            Id = 11
                        },
                        Code = "19003",
                        Name = "خودرو",
                        Description = "خودرو"
                    }

                }));

                return logList;
            });
        }
        public Task<ResultViewModel> TransferLogBulk(List<Log> logs, string token = default)
        {
            return Task.Run(async () =>
            {
                try
                {
                    return await _logRepository.BroadcastLogs(logs, token);
                    //var deviceCodes = logs.GroupBy(g => g.DeviceCode).Select(s => s.Key).ToList();
                    //foreach (var deviceCode in deviceCodes)
                    //{
                    //    var deviceLogs = logs.Where(x => x.DeviceCode == deviceCode).OrderByDescending(x => x.LogDateTime).ToList();
                    //    //برای ارسال لاگ به صورت 1000 تا 1000تا
                    //    var logsCount = deviceLogs.Count;
                    //    var loopUpperBound = logsCount / 1000;
                    //    loopUpperBound = loopUpperBound == 0 ? 1 : loopUpperBound;
                    //    loopUpperBound = logsCount % 1000 <= 0 ? loopUpperBound : loopUpperBound + 1;

                    //    for (var i = 0; i < loopUpperBound; i++)
                    //    {
                    //        var shortenedLogList = deviceLogs.Skip(i * 1000).Take(1000).ToList();

                    //        var restRequest = new RestRequest("UpdateAttendance/UpdateAttendanceBulk", Method.POST, DataFormat.Json);
                    //        restRequest.AddJsonBody(shortenedLogList);
                    //        restRequest.AddHeader("Authorization", token);

                    //        var result = await _restClient.ExecuteAsync<ResultViewModel>(restRequest);

                    //        //integration
                    //        await _logMessageBusRepository.SendLog(logs.Where(log => !log.SuccessTransfer).ToList());

                    //        //var biovationBrokerMessageData = new List<DataChangeMessage<Log>>
                    //        //      {
                    //        //     new DataChangeMessage<Log>
                    //        //      {
                    //        //          Id = Guid.NewGuid().ToString(), EventId = 1, SourceName = "BiovationCore",
                    //        //          TimeStamp = DateTimeOffset.Now, SourceDatabaseName = "biovation", Data = shortenedLogList
                    //        //          }
                    //        //      };

                    //        //_biovationInternalSource.PushData(biovationBrokerMessageData);

                    //        if (!result.IsSuccessful || result.StatusCode != HttpStatusCode.OK || result.Data.Validate != 1) continue;
                    //        shortenedLogList.ForEach(log => log.SuccessTransfer = true);
                    //        await UpdateLog(shortenedLogList);
                    //    }
                    //}

                    //return new ResultViewModel { Validate = 1 };
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
