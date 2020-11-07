using Biovation.CommonClasses;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Biovation.Constants;
using Biovation.Domain;
using Biovation.Service.Api.v2;

namespace Biovation.Brands.EOS.Service
{
    /// <summary>
    /// کلاس مربوط به ایجاد امکان استفاده از یک ریپوزیتوری برای انتقال داده های گزارش ها به دیتابیس
    /// </summary>
    public class EosLogService
    {
        private readonly LogService _logService;
        private readonly DeviceService _deviceService;

        public EosLogService(LogService logService, DeviceService deviceService)
        {
            _logService = logService;
            _deviceService = deviceService;
        }

        /// <summary>
        /// <En>Call a repository method to insert detailed infotmation of a log data into database.</En>
        /// <Fa>با صدا کردن یک تابع در ریپوزیتوری اطلاعات یک گزارش از ساعت را در دیتابیس ثبت میکند.</Fa>
        /// </summary>
        /// <param name="log">اطلاعات گزارش</param>
        /// <returns></returns>
        public Task<ResultViewModel> AddLog(Log log)
        {
            return Task.Run(async () =>
            {

                //log.MatchingType =  log.MatchingType;

                var device = _deviceService.GetDevice(log.DeviceId);
                log.InOutMode = device?.Data?.DeviceTypeId ?? 0;
                var addLogResult = await _logService.AddLog(log);

                return addLogResult;
            });
        }



        public Task<ResultViewModel> AddLog(List<Log> logs)
        {
            return Task.Run(async () =>
            {
                try
                {
                    var addLogResult = await _logService.AddLog(logs);

                    if (addLogResult.Validate == 1)
                    {
                        await Task.Run(async () =>
                        {
                            var deviceCodes = logs.GroupBy(g => g.DeviceCode).Select(s => s.Key).ToList();
                            foreach (var deviceCode in deviceCodes)
                            {
                                var device = _deviceService.GetDevices(code: deviceCode, brandId: DeviceBrands.EosCode)?.Data?.Data?.FirstOrDefault();
                                if (device != null)
                                {
                                    var logsToTransfer = await _logService.SelectSearchedOfflineLogs(new DeviceTraffic { DeviceId = (uint)device.DeviceId, State = false });

                                    //var deviceLogs = logs.Where(x => x.DeviceCode == deviceCode).ToList();

                                    logsToTransfer.ForEach(x =>
                                    {
                                        x.InOutMode = device.DeviceTypeId;
                                        x.DeviceId = device.DeviceId;
                                    });

                                    await Task.Run(async () =>
                                    {
                                        try
                                        {
                                            var result = await _logService.TransferLogBulk(logsToTransfer);

                                            if (result.Validate == 1)
                                            {
                                                logsToTransfer.ForEach(x => { x.SuccessTransfer = true; });
                                                await _logService.UpdateLog(logsToTransfer);
                                            }
                                        }
                                        catch (Exception e)
                                        {
                                            Logger.Log(e.Message, "<--  --- ERROR ON Transfer Attendance :");
                                        }
                                    });
                                }
                            }
                        });
                    }

                    return addLogResult;
                }
                catch (Exception e)
                {
                    Logger.Log(e.Message, "<--  --- ERROR ON AddLog Batch :");
                    return new ResultViewModel { Validate = 0 };
                }
            });
        }
    }
}
