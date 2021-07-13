using Biovation.Constants;
using Biovation.Domain;
using Biovation.Service.Api.v1;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace Biovation.Brands.ZK.Controllers
{
    [ApiController]
    [Route("Biovation/Api/[controller]/[action]")]
    public class LogController : ControllerBase
    {
        private readonly DeviceService _deviceService;

        public LogController( DeviceService deviceService)
        {
            _deviceService = deviceService;
        }

        [HttpPost]
        [Authorize]
        public async Task<ResultViewModel> ClearLog(uint code, DateTime? fromDate, DateTime? toDate)
        {
            return await Task.Run(() =>
            {
                try
                {
                    var device = _deviceService.GetDevices(code: code, brandId: DeviceBrands.ZkTecoCode).FirstOrDefault();
                    if (device is null)
                        return new ResultViewModel { Success = false, Message = $"Device {code} does not exists" };



                    return new ResultViewModel { Validate = 1, Message = "Clear LOg queued" };
                }
                catch (Exception exception)
                {
                    return new ResultViewModel { Validate = 0, Message = exception.ToString() };
                }
            });
        }
    }
}