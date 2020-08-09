using Biovation.CommonClasses.Manager;
using Biovation.CommonClasses.Models;
using Biovation.CommonClasses.Models.ConstantValues;
using Biovation.CommonClasses.Repository;
using Newtonsoft.Json;
using RestSharp;
using System;
using System.Collections.Generic;
using System.Data;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.Net;
using System.Threading.Tasks;

namespace Biovation.CommonClasses.Service
{
    public class LogService
    {
        private readonly LogRepository _logRepository = new LogRepository();
        private readonly RestClient _logExternalSubmissionRestClient;

        //private readonly PlateDetectionRepository _plateDetectionRepository = new PlateDetectionRepository();
        public LogService()
        {
            _logExternalSubmissionRestClient = (RestClient)new RestClient(ConfigurationManager.LogMonitoringApiUrl).UseSerializer(() => new RestRequestJsonSerializer());
        }

        public Task<ResultViewModel> AddLog(Log log)
        {
            return Task.Run(async () =>
            {
                try
                {
                    var insertionResult = await _logRepository.AddLog(log);

                    if (log.EventLog.Code == LogEvents.Authorized.Code || log.EventLog.Code == LogEvents.UnAuthorized.Code)
                    {
                        await Task.Run(async () =>
                        {
                            try
                            {
                                for (var i = 0; i < 3; i++)
                                {
                                    var restRequest = new RestRequest("UpdateAttendance/UpdateAttendance", Method.POST);
                                    restRequest.AddJsonBody(log);

                                    var result = await _logExternalSubmissionRestClient.ExecuteTaskAsync<ResultViewModel>(restRequest);

                                    log.SuccessTransfer = result.IsSuccessful && result.StatusCode == HttpStatusCode.OK && result.Data?.Validate == 1;
                                    if (!log.SuccessTransfer) continue;

                                    await UpdateLog(log);
                                    break;
                                }
                            }
                            catch (Exception exception)
                            {
                                Logger.Log(exception);
                            }

                            try
                            {
                                // صفحه مانیتورینگ
                                var restRequest = new RestRequest("UpdateMonitoring/UpdateMonitoring", Method.POST);
                                restRequest.AddJsonBody(ConfigurationManager.ShowLiveImageInMonitoring
                                    ? log
                                    : new Log
                                    {
                                        Id = log.Id,
                                        DeviceId = log.DeviceId,
                                        DeviceCode = log.DeviceCode,
                                        UserId = log.UserId,
                                        LogDateTime = log.LogDateTime,
                                        EventLog = log.EventLog,
                                        AuthResult = log.AuthResult,
                                        AuthType = log.AuthType,
                                        InOutMode = log.InOutMode,
                                        DeviceName = log.DeviceName,
                                        MatchingType = log.MatchingType,
                                        SubEvent = log.SubEvent,
                                        SurName = log.SurName,
                                        TnaEvent = log.TnaEvent
                                    });
                                await _logExternalSubmissionRestClient.ExecuteTaskAsync(restRequest);
                            }
                            catch (Exception exception)
                            {
                                Logger.Log(exception);
                            }
                        });
                    }

                    return insertionResult;
                }
                catch (Exception exception)
                {
                    Logger.Log(exception);
                    return new ResultViewModel { Validate = 0, Message = exception.ToString() };
                }
            });
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

        /*public Task<ResultViewModel> AddLog(DataTable logs)
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
        }*/

        //public ResultViewModel UpdateAttendanceBulk(List<Log> logs)
        //{
        //    try
        //    {
        //        var deviseIds = logs.GroupBy(g => g.DeviceCode).Select(s => s.Key).ToList();
        //        foreach (var item in deviseIds)
        //        {
        //            var device = _commonDeviceService.GetDeviceBasicInfoWithCode((uint)item, DeviceBrands.Virdi);

        //            var lstLogs = logs.Where(x => x.DeviceCode == item).ToList();

        //            lstLogs.ForEach(x => { x.DeviceIOType = device.DeviceTypeId; x.DeviceId = device.DeviceId; });

        //                var data = JsonConvert.SerializeObject(lstLogs);
        //                try
        //                {
        //                    var result = _communicationManager.CallRest("/api/Biovation/UpdateAttendance/UpdateAttendanceBulk", "Post", null, data);
        //                    if (result.Validate == 1)
        //                    {
        //                        //lstLogs.ForEach(x => { x.SuccessTransfer = true; });
        //                        var jsonData = JsonConvert.SerializeObject(lstLogs.Select(s => new
        //                        {
        //                            s.Id,
        //                            s.DeviceId,
        //                            s.DeviceCode,
        //                            s.EventId,
        //                            s.UserId,
        //                            datetime = s.LogDateTime,
        //                            Ticks = s.DateTimeTicks,
        //                            s.SubEvent,
        //                            TNAEvent = s.TnaEvent,
        //                            s.MatchingType,
        //                            SuccessTransfer = true
        //                        }).ToList());
        //                        var logDataTable = JsonConvert.DeserializeObject<DataTable>(jsonData);
        //                        _commonLogService.UpdateLog(logDataTable);
        //                    }
        //                }
        //                catch (Exception)
        //                {
        //                    //ignore
        //                }                  

        //        }

        //    }
        //    catch(Exception e)
        //    {
        //        return new ResultViewModel { Validate = 0 };
        //    }    
        //}



        public Task<ResultViewModel> AddLogBulk(List<Log> logs)
        {
            return Task.Run(async () =>
            {
                try
                {
                    //return logRepository.AddLogBulk(logs);

                    var serializedLogs = JsonConvert.SerializeObject(logs.Select(s => new
                    {
                        s.Id,
                        s.DeviceId,
                        s.DeviceCode,
                        EventId = s.EventLog.Code,
                        s.UserId,
                        datetime = s.LogDateTime,
                        Ticks = s.DateTimeTicks,
                        SubEvent = s.SubEvent?.Code,
                        TNAEvent = s.TnaEvent,
                        s.InOutMode,
                        s.MatchingType,
                        s.SuccessTransfer
                    }));

                    var logsDataTable = JsonConvert.DeserializeObject<DataTable>(serializedLogs);
                    return await _logRepository.AddLog(logsDataTable);
                }
                catch (Exception)
                {
                    try
                    {
                        foreach (var log in logs)
                        {
                            await AddLog(log);
                        }

                        return new ResultViewModel { Validate = 1 };
                    }
                    catch (Exception)
                    {
                        return new ResultViewModel { Validate = 0 };
                    }
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

        public Task<List<Log>> CheckLogInsertion(List<Log> logs)
        {
            return Task.Run(async () =>
            {
                try
                {
                    return await _logRepository.CheckLogInsertion(logs);
                }
                catch (Exception)
                {
                    return new List<Log>();
                }
            });
        }

        public Task<List<Log>> GetOfflineLogs()
        {


            return Task.Run(() =>
            {
                return Task.Run(() => _logRepository.GetOfflineLogs());

                /*var getPlateLogTask = Task.Run(() => _plateDetectionRepository.GetPlateDetectionLog());
                //mapping
                var convertLog =getPlateLogTask.Result.Select(platelog => new Log()
                {
                    
                    DeviceId = platelog.DetectorId,
                    UserId = platelog.LicensePlate.EntityId,
                    EventLog = platelog.EventLog,
                    DateTimeTicks = platelog.DateTimeTicks,
                    LogDateTime = platelog.LogDateTime,
                    PicByte = platelog.PlateImage,
                    SuccessTransfer = platelog.SuccessTransfer,

                }).ToList();
                getLogTask.Result.AddRange(convertLog);*/
                //return getLogTask.Result;
            });
        }

        public Task<List<Log>> GetLastLogsOfDevice(uint deviceId)
        {
            return Task.Run(async () => await _logRepository.GetLastLogsOfDevice(deviceId));
        }

        public Task<List<Log>> GetOfflineLogsByDeviceId(uint deviceId)
        {
            return Task.Run(async () => await _logRepository.GetOfflineLogsByDeviceId(deviceId));
        }

        public Task<List<Log>> GetOfflineLogsByUserId(int userId)
        {
            return Task.Run(async () => await _logRepository.GetOfflineLogsByUserId(userId));
        }

        public Task<List<Log>> GetOfflineLogsOfPeriod(DateTime fromDate, DateTime toDate)
        {
            return Task.Run(async () => await _logRepository.GetOfflineLogsOfPeriod(fromDate, toDate));
        }
        public Task<List<Log>> SelectSearchedOfflineLogs(DeviceTraffic deviceTraffic)
        {
            return Task.Run(async () => await _logRepository.SelectSearchedOfflineLogs(deviceTraffic));
        }
        public Task<List<Log>> SelectSearchedOfflineLogs(int userId = default, int deviceId = default, DateTime fromDate = default, DateTime toDate = default, int onlineUserId = 123456789, bool? state = null)
        {
            var deviceTraffic = new DeviceTraffic
            {
                UserId = userId,
                DeviceId = (uint)deviceId,
                FromDate = fromDate == default ? new DateTime(1990, 1, 1) : fromDate,
                ToDate = toDate == default ? new DateTime(2050, 1, 1) : toDate,
                OnlineUserId = onlineUserId,
                State = state
            };

            return Task.Run(async () => await _logRepository.SelectSearchedOfflineLogs(deviceTraffic));
        }
        public Task<List<Log>> SelectSearchedOfflineLogsWithPaging(DeviceTraffic deviceTraffic)
        {
            return Task.Run(async () => await _logRepository.SelectSearchedOfflineLogsWithPaging(deviceTraffic));
        }
        public Task<List<Log>> GetOfflineLogsOfPeriodByDeviceId(uint deviceId, DateTime fromDate, DateTime toDate)
        {
            return Task.Run(async () => await _logRepository.GetOfflineLogsOfPeriodByDeviceId(deviceId, fromDate, toDate));
        }

        public Task<List<Log>> GetOfflineLogsOfPeriodByUserId(int userId, DateTime fromDate, DateTime toDate)
        {
            return Task.Run(async () => await _logRepository.GetOfflineLogsOfPeriodByUserId(userId, fromDate, toDate));
        }

        public Task<ResultViewModel> UpdateLog(Log log)
        {
            return Task.Run(async () => await _logRepository.UpdateLog(log));
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

        public Task<ResultViewModel> AddLogImage(Log log)
        {
            return Task.Run(async () => await _logRepository.AddLogImage(log));
        }

        public Task<string> SaveImage(byte[] image, long userId, DateTime logDatetime, uint deviceCode, string brandName)
        {
            return Task.Run(() =>
            {
                try
                {
                    var directory = Path.Combine(AppDomain.CurrentDomain.BaseDirectory,
                        $@"\LogPic\{brandName}\{logDatetime.Year}\{logDatetime.Month}\{logDatetime.Day}");
                    if (!Directory.Exists(directory))
                    {
                        Directory.CreateDirectory(directory);
                    }

                    var fileName =
                        $"{userId}-{logDatetime:HH-mm-ss}({deviceCode}).jpg";
                    using (var ms = new MemoryStream(image))
                    {
                        using var img = Image.FromStream(ms);
                        img.Save(Path.Combine(directory, fileName), ImageFormat.Jpeg);
                    }

                    return Path.Combine(directory, fileName);
                }
                catch (Exception exception)
                {
                    Logger.Log(exception);
                    return default;
                }
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

        public Task<byte[]> GetImage(long id)
        {
            return Task.Run(async () =>
            {
                var log = await _logRepository.GetLog(id);
                if (log == null || string.IsNullOrEmpty(log.Image)) return new byte[0];
                var path = log.Image;
                var bytes = File.ReadAllBytes(path);
                return bytes;
            });
        }
    }
}
