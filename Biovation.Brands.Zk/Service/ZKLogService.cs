﻿using Biovation.CommonClasses;
using Biovation.Constants;
using Biovation.Domain;
using Biovation.Service.Api.v1;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Biovation.Brands.ZK.Service
{
    public class ZkLogService
    {
        private readonly DeviceService _deviceService;
        private readonly LogService _logService;

        public ZkLogService(DeviceService deviceService, LogService logService)
        {
            _deviceService = deviceService;
            _logService = logService;
        }

        public Task<ResultViewModel> AddLog(Log log)
        {
            return Task.Run(async () =>
            {
                /*var authmode = _commonDeviceService.GetBioAuthModeWithDeviceId(log.DeviceId, log.MatchingType);
                 log.MatchingType = authMode?.BioCode ?? log.MatchingType;
                 */
                var device = _deviceService.GetDevices(code: log.DeviceCode, brandId: DeviceBrands.ZkTecoCode).FirstOrDefault();
                log.InOutMode = device?.DeviceTypeId ?? 0;
                var addLogResult = await _logService.AddLog(log);
                //Thread.Sleep(200);
                return addLogResult;
            });
        }

        public Task<ResultViewModel> AddLog(List<Log> logs)
        {
            return Task.Run(async () =>
            {
                try
                {
                    /*ar logsAuthenticationTypes = logs.GroupBy(g => g.MatchingType).Select(s => s.Key).ToList();
                     foreach (var logsAuthenticationType in logsAuthenticationTypes)
                     {
                         var authMode = _commonDeviceService.GetBioAuthModeWithDeviceId(logs.FirstOrDefault()?.DeviceId ?? 0, logsAuthenticationType);
                         if (authMode is null)
                             continue;

                         logs.Where(x => x.MatchingType == logsAuthenticationType).ToList().ForEach(log =>
                         {
                             log.MatchingType = authMode.BioCode;
                         });
                     }*/

                    var deviceCodes = logs.GroupBy(g => g.DeviceCode).Select(s => s.Key).Where(s => s != 0).ToList();
                    foreach (var deviceCode in deviceCodes)
                    {
                        var device = _deviceService.GetDevices(code: deviceCode, brandId: DeviceBrands.ZkTecoCode).FirstOrDefault();

                        logs.Where(x => x.DeviceCode == deviceCode).ToList().ForEach(x =>
                                {
                                    x.InOutMode = device?.DeviceTypeId ?? 0;
                                    x.DeviceId = device?.DeviceId ?? -5;
                                });
                    }

                    var addLogResult = await _logService.AddLog(logs);
                    if (addLogResult.Validate != 1) return addLogResult;

                    foreach (var deviceCode in deviceCodes)
                    {
                        var device = _deviceService.GetDevices(code: deviceCode, brandId: DeviceBrands.ZkTecoCode).FirstOrDefault();
                        //var logsToTransfer = await _logService.SelectSearchedOfflineLogs(deviceId: device.DeviceId, state: false);
                        if (device?.DeviceId != null)
                        {
                            var logsToTransfer = await _logService.SelectSearchedOfflineLogs(new DeviceTraffic() { DeviceId = (uint)device?.DeviceId, State = false });
                            await Task.Run(() => { _logService.TransferLogBulk(logsToTransfer); });

                            await Task.Run(async () =>
                            {
                                var logsWithImages = logs.Where(log => logsToTransfer.Any(newLog =>
                                    log.UserId == newLog.UserId && log.LogDateTime == newLog.LogDateTime &&
                                    log.EventLog.Code == newLog.EventLog.Code && log.DeviceId == newLog.DeviceId) && log.PicByte?.Length > 0).ToList();

                                foreach (var log in logsWithImages)
                                {
                                    //Todo: Change for .net core
                                    //var filePath = log.PicByte is null
                                    //    ? string.Empty
                                    //    : await _logService.SaveImage(log.PicByte, log.UserId, log.LogDateTime, log.DeviceCode, DeviceBrands.ZkTeco.Name);
                                    //log.Image = filePath;

                                    //await _logService.AddLogImage(log);
                                }
                            });
                        }
                    }
                    return addLogResult;

                }
                catch (Exception exception)
                {
                    Logger.Log(exception, "<-- --- ERROR ON add Log Batch :");
                    return new ResultViewModel { Validate = 0, Message = exception.ToString() };
                }
            });
        }
    }
}
