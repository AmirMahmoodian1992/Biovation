using Biovation.Domain;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Threading.Tasks;

namespace Biovation.Brands.ZK.Controllers
{
    [ApiController]
    [Route("Biovation/Api/[controller]/[action]")]
    public class ZkTimeZoneController : ControllerBase
    {
        [HttpPost]
        [Authorize]
        public async Task<ResultViewModel> SendTimeZoneToAllDevices([FromBody] int timeZoneId)
        {
            return await Task.Run(() =>
            {
                try
                {
                    return new ResultViewModel { Validate = 1, Message = "Sending TimeZoneToTerminal queued" };
                }
                catch (Exception exception)
                {
                    return new ResultViewModel
                    {
                        Validate = 0,
                        Message = exception.ToString()
                    };
                }
            });

        }

        [HttpGet]
        [Authorize]
        public async Task<ResultViewModel> SendTimeZoneToDevice(int timeZoneId, uint code)
        {
            return await Task.Run(() =>
                {
                    try
                    {

                        //_taskManager.ProcessQueue();
                        return new ResultViewModel { Validate = 1, Message = "Sending TimeZoneToTerminal queued" };
                    }
                    catch (Exception exception)
                    {
                        return new ResultViewModel
                        {
                            Validate = 0,
                            Message = exception.ToString()
                        };
                    }
                });
        }
    }
}
