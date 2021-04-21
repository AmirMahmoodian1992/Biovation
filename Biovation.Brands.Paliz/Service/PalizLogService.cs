using Biovation.CommonClasses;
using Biovation.Constants;
using Biovation.Domain;
using Biovation.Service.Api.v2;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Biovation.Brands.Paliz.Service
{
    public class PalizLogService
    {
        private readonly LogService _commonLogService;
        private readonly DeviceService _commonDeviceService;

        public PalizLogService(DeviceService commonDeviceService, LogService commonLogService)
        {
            _commonLogService = commonLogService;
            _commonDeviceService = commonDeviceService;
        }

        public async Task<ResultViewModel> AddLog(Log log)
        {

            var deviceViewModel = _commonDeviceService.GetDevices(code: log.DeviceCode, brandId: DeviceBrands.VirdiCode);
            var device = deviceViewModel?.Data?.Data.FirstOrDefault();
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
                    var deviceViewModel = _commonDeviceService.GetDevices(code: deviceCode, brandId: DeviceBrands.VirdiCode);
                    var device = deviceViewModel?.Data?.Data.FirstOrDefault();
                    logs.Where(x => x.DeviceCode == deviceCode).ToList().ForEach(x =>
                    {
                        x.InOutMode = device?.DeviceTypeId ?? 0;
                        x.DeviceId = device?.DeviceId ?? -1;
                    });
                }

                return await _commonLogService.AddLog(logs);
            }
            catch (Exception exception)
            {
                Logger.Log(exception, "<-- --- ERROR ON add Log Batch :");
                return new ResultViewModel { Validate = 0, Message = exception.ToString() };
            }
        }
    }
}
