using Biovation.CommonClasses;
using Biovation.Constants;
using Biovation.Domain;
using Biovation.Service.Api.v1;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Biovation.Brands.Suprema.Services
{   
    public class SupremaLogService
    {
        private readonly LogService _logService;
        private readonly DeviceService _deviceService;
        private readonly MatchingTypes _matchingTypes;

        public SupremaLogService(LogService commonLogService, DeviceService commonDeviceService, MatchingTypes matchingTypes)
        {
            _logService = commonLogService;
            _deviceService = commonDeviceService;
            _matchingTypes = matchingTypes;
        }

        public Task<ResultViewModel> AddLog(Log log)
        {
            return Task.Run(async () =>
            {
                var device = _deviceService.GetDevices(code: log.DeviceCode, brandId: DeviceBrands.VirdiCode)?.FirstOrDefault();
                // var authMode = _commonDeviceService.GetBioAuthModeWithDeviceId(log.DeviceId, log.MatchingType);

                // log.MatchingType = authMode?.BioCode;
                //log.MatchingType = log.MatchingType ?? MatchingTypes.Unknown;
                log.DeviceId = device?.DeviceId ?? log.DeviceId;
                //var commonLogRepository = new LogRepository();

                //todo:save image
                //var filePath = log.PicByte is null
                //    ? string.Empty
                //    : _logService.SaveImage(log.PicByte, log.UserId, log.LogDateTime, log.DeviceCode, DeviceBrands.Virdi.Name).Result;
                //log.Image = filePath;

                log.InOutMode = device?.DeviceTypeId ?? 0;
                if (log.EventLog.Code == "16001" || log.EventLog.Code == "16002" || log.EventLog.Code == "16007")
                {
                    log.UserId = 0;
                }

                return await _logService.AddLog(log);
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
                        log.MatchingType ??= _matchingTypes.Unknown;
                    }
                    var deviceCodes = logs.GroupBy(g => g.DeviceCode).Select(s => s.Key).Where(s => s != 0).ToList();
                    foreach (var deviceCode in deviceCodes)
                    {
                        var device = _deviceService.GetDevices(code: deviceCode, brandId: DeviceBrands.SupremaCode)?.FirstOrDefault();
                        logs.Where(x => x.DeviceCode == deviceCode).ToList().ForEach(x =>
                        {
                            x.InOutMode = device?.DeviceTypeId ?? 0;
                            x.DeviceId = device?.DeviceId ?? -5;
                        });
                    }

                    var addLogResult = await _logService.AddLog(logs);

                    if (addLogResult.Validate != 1) return addLogResult;

                    await Task.Run(() => { _logService.TransferLogBulk(logs); });
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
            //todo:check new method functionality
            //return _logService.GetLastLogsOfDevice(deviceId);
            return _logService.Logs(deviceId: (int)deviceId, pageNumber: 1, pageSize: 5);
        }
    }
}
