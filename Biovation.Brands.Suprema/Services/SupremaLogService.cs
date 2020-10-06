using Biovation.CommonClasses;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Biovation.Constants;
using Biovation.Domain;
using Biovation.Service.Api.v1;

namespace Biovation.Brands.Suprema.Service
{
    public class SupremaLogService
    {
        private readonly DeviceService _commonDeviceService = new DeviceService();
        private readonly LogService _commonLogService = new LogService();

        public Task<ResultViewModel> AddLog(Log log)
        {
            return Task.Run(async () =>
            {
                var device = _commonDeviceService.GetDeviceBasicInfoWithCode(log.DeviceCode, DeviceBrands.VirdiCode);
                // var authMode = _commonDeviceService.GetBioAuthModeWithDeviceId(log.DeviceId, log.MatchingType);

                // log.MatchingType = authMode?.BioCode;
                //log.MatchingType = log.MatchingType ?? MatchingTypes.Unknown;
                log.DeviceId = device?.DeviceId ?? log.DeviceId;
                //var commonLogRepository = new LogRepository();
                var filePath = log.PicByte is null
                    ? string.Empty
                    : _commonLogService.SaveImage(log.PicByte, log.UserId, log.LogDateTime, log.DeviceCode, DeviceBrands.Virdi.Name).Result;
                log.Image = filePath;
                log.InOutMode = device?.DeviceTypeId ?? 0;
                if (log.EventLog.Code == "16001" || log.EventLog.Code == "16002" || log.EventLog.Code == "16007")
                {
                    log.UserId = 0;
                }

                return await _commonLogService.AddLog(log);
            });
        }

        public Task<ResultViewModel> AddLog(List<Log> logs)
        {
            return Task.Run(async () =>
            {
                try
                {
                    foreach (var log in logs)
                    {
                        log.MatchingType = log.MatchingType ?? MatchingTypes.Unknown;
                    }
                    var deviceCodes = logs.GroupBy(g => g.DeviceCode).Select(s => s.Key).Where(s => s != 0).ToList();
                    foreach (var deviceCode in deviceCodes)
                    {
                        var device = _commonDeviceService.GetDeviceBasicInfoWithCode(deviceCode, DeviceBrands.SupremaCode);
                        logs.Where(x => x.DeviceCode == deviceCode).ToList().ForEach(x =>
                        {
                            x.InOutMode = device?.DeviceTypeId ?? 0;
                            x.DeviceId = device?.DeviceId ?? -5;
                        });
                    }

                    var addLogResult = await _commonLogService.AddLog(logs);

                    if (addLogResult.Validate != 1) return addLogResult;

                    await Task.Run(() => { _commonLogService.TransferLogBulk(logs); });
                    return addLogResult;

                }
                catch (Exception exception)
                {
                    Logger.Log(exception, "<-- --- ERROR ON add Log Batch :");
                    return new ResultViewModel { Validate = 0, Message = exception.ToString() };
                }
            });
        }

        public Task<List<Log>> GetLastLogsOfDevice(uint deviceId)
        {
            return _commonLogService.GetLastLogsOfDevice(deviceId);
        }
    }
}
