using Biovation.CommonClasses;
using Biovation.Constants;
using Biovation.Domain;
using Biovation.Service.Api.v1;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Biovation.Brands.Virdi.Service
{
    public class VirdiLogService
    {
        private readonly LogService _commonLogService;
        private readonly DeviceService _commonDeviceService;

        public VirdiLogService(DeviceService commonDeviceService, LogService commonLogService)
        {
            _commonLogService = commonLogService;
            _commonDeviceService = commonDeviceService;
        }

        public async Task<ResultViewModel> AddLog(Log log)
        {

            var device = _commonDeviceService.GetDevices(code: log.DeviceCode, brandId: DeviceBrands.VirdiCode).FirstOrDefault();
            log.DeviceId = device?.DeviceId ?? log.DeviceId;
            log.InOutMode = device?.DeviceTypeId ?? 0;

            return await _commonLogService.AddLog(log);
        }

        public async Task<ResultViewModel> AddLog(List<Log> logs)
        {
            try
            {
                foreach (var deviceCode in logs.GroupBy(g => g.DeviceCode).Select(s => s.Key).Where(s => s != 0))
                {
                    var device = _commonDeviceService.GetDevices(code: deviceCode, brandId: DeviceBrands.VirdiCode).FirstOrDefault();
                    logs.Where(x => x.DeviceCode == deviceCode).ToList().ForEach(x =>
                    {
                        x.InOutMode = device?.DeviceTypeId ?? 0;
                        x.DeviceId = device?.DeviceId ?? -1;
                    });
                }

                return await _commonLogService.AddLog(logs);
                //if (addLogResult.Validate != 1) return addLogResult;

                //foreach (var deviceCode in deviceCodes)
                //{
                //    var device = _commonDeviceService.GetDevices(code: deviceCode, brandId: DeviceBrands.VirdiCode).FirstOrDefault();
                //    var logsToTransfer = await _commonLogService.Logs(deviceId: device.DeviceId);
                //    await Task.Run(() => { _commonLogService.TransferLogBulk(logsToTransfer); });

                //    await Task.Run(async () =>
                //    {
                //        var logsWithImages = logs.Where(log => logsToTransfer.Any(newLog =>
                //            log.UserId == newLog.UserId && log.LogDateTime == newLog.LogDateTime &&
                //            log.EventLog.Code == newLog.EventLog.Code && log.DeviceId == newLog.DeviceId) && log.PicByte?.Length > 0).ToList();

                //        foreach (var log in logsWithImages)
                //        {
                //            //Todo: Change for .net core
                //            //var filePath = log.PicByte is null
                //            //    ? string.Empty
                //            //    : await _commonLogService.SaveImage(log.PicByte, log.UserId, log.LogDateTime, log.DeviceCode, DeviceBrands.Virdi.Name);
                //            //log.Image = filePath;

                //            await _commonLogService.AddLogImage(log);
                //        }
                //    });
                //}
                //return addLogResult;

            }
            catch (Exception exception)
            {
                Logger.Log(exception, "<-- --- ERROR ON add Log Batch :");
                return new ResultViewModel { Validate = 0, Message = exception.ToString() };
            }
        }
    }
}