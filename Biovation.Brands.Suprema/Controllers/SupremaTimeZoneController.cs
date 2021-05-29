using Biovation.Domain;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Threading.Tasks;

namespace Biovation.Brands.Suprema.Controllers
{
    [ApiController]
    [Route("Biovation/Api/[controller]/[action]")]
    public class SupremaTimeZoneController : ControllerBase
    {

        [HttpPost]
        [Authorize]
        public Task<ResultViewModel> SendTimeZoneToAllDevices([FromBody] int timeZoneId)
        {
            return Task.Run(() => { 
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
        public Task<ResultViewModel> SendTimeZoneToDevice(int timeZoneId, uint code)
        {
            return Task.Run(() =>
            {
                try
                {
                    return new ResultViewModel {Validate = 1, Message = "Sending TimeZoneToTerminal queued"};
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
