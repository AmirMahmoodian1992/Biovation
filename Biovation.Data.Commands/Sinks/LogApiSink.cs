using Biovation.CommonClasses;
using Biovation.CommonClasses.Manager;
using Biovation.Domain;
using Biovation.Repository.Sql.v2;
using RestSharp;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading.Tasks;

namespace Biovation.Data.Commands.Sinks
{
    public class LogApiSink
    {
        private readonly RestClient _restClient;
        private readonly LogRepository _logRepository;
        private readonly BiovationConfigurationManager _biovationConfigurationManager;
        public LogApiSink(LogRepository logRepository, BiovationConfigurationManager biovationConfigurationManager)
        {
            _logRepository = logRepository;
            _biovationConfigurationManager = biovationConfigurationManager;
            _restClient = (RestClient)new RestClient(biovationConfigurationManager.BiovationServerUri).UseSerializer(() => new RestRequestJsonSerializer());
        }

        public Task<ResultViewModel> TransferLog(Log log)
        {
            return Task.Run(async () =>
            {
                if (!_biovationConfigurationManager.BroadcastToApi)
                    return new ResultViewModel { Success = false, Message = "The Api broadcast option is off" };

                try
                {
                    try
                    {
                        // صفحه مانیتورینگ
                        var restRequest = new RestRequest("UpdateMonitoring/UpdateMonitoring", Method.POST);
                        restRequest.AddJsonBody(_biovationConfigurationManager.ShowLiveImageInMonitoring
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
                        await _restClient.ExecuteAsync(restRequest);

                    }
                    catch (Exception exception)
                    {
                        Logger.Log(exception);
                    }

                    for (var i = 0; i < 3; i++)
                    {
                        var restRequest = new RestRequest("UpdateAttendance/UpdateAttendance", Method.POST);
                        restRequest.AddJsonBody(log);

                        var result = await _restClient.ExecuteAsync<ResultViewModel>(restRequest);

                        log.SuccessTransfer = result.IsSuccessful && result.StatusCode == HttpStatusCode.OK && result.Data?.Validate == 1;
                        if (!log.SuccessTransfer) continue;

                        await _logRepository.UpdateLog(log);
                        break;
                    }

                    return new ResultViewModel { Success = log.SuccessTransfer, Message = log.SuccessTransfer ? "Logs are sent successfully." : "An error occured" };
                }
                catch (Exception exception)
                {
                    Logger.Log(exception);
                    return new ResultViewModel { Success = false, Message = exception.Message };
                }
            });
        }

        public Task<ResultViewModel> TransferLogBulk(List<Log> logs)
        {
            return Task.Run(async () =>
            {
                try
                {
                    if (!_biovationConfigurationManager.BroadcastToApi)
                        return new ResultViewModel { Success = false, Message = "The Api broadcast option is off" };

                    foreach (var deviceCode in logs.GroupBy(g => g.DeviceCode).Select(s => s.Key))
                    {
                        var deviceLogs = logs.Where(x => x.DeviceCode == deviceCode).OrderByDescending(x => x.LogDateTime).ToList();
                        //برای ارسال لاگ به صورت 1000 تا 1000تا
                        var logsCount = deviceLogs.Count();
                        var loopUpperBound = logsCount / 1000;
                        loopUpperBound = loopUpperBound == 0 ? 1 : loopUpperBound;
                        loopUpperBound = logsCount % 1000 <= 0 ? loopUpperBound : loopUpperBound + 1;

                        for (var i = 0; i < loopUpperBound; i++)
                        {
                            var shortenedLogList = deviceLogs.Skip(i * 1000).Take(1000).ToList();

                            var restRequest = new RestRequest("UpdateAttendance/UpdateAttendanceBulk", Method.POST);
                            restRequest.AddJsonBody(shortenedLogList);

                            var result = await _restClient.ExecuteAsync<ResultViewModel>(restRequest);
                            if (!result.IsSuccessful || result.StatusCode != HttpStatusCode.OK || result.Data.Validate != 1) continue;

                            shortenedLogList.ForEach(log => log.SuccessTransfer = true);
                            await _logRepository.UpdateLog(shortenedLogList);
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
