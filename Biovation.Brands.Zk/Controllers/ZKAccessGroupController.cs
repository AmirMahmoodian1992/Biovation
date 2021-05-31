using Biovation.Domain;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Threading.Tasks;

namespace Biovation.Brands.ZK.Controllers
{
    [ApiController]
    [Route("Biovation/Api/[controller]/[action]")]
    public class AccessGroupController : ControllerBase
    {
        [HttpPost]
        [Authorize]
        public Task<ResultViewModel> SendAccessGroupToAllDevices([FromBody] int accessGroupId)
        {
            return Task.Run(() =>
            {
                try
                {
                    return new ResultViewModel { Validate = 1, Message = "Sending AccessGroupToTerminal queued" };
                }
                catch (Exception exception)
                {
                    return new ResultViewModel { Validate = 0, Message = exception.ToString() };
                }
            });
        }

        [HttpGet]
        [Authorize]
        public Task<ResultViewModel> SendAccessGroupToDevice(int accessGroupId, uint code)
        {
            return Task.Run(() =>
            {
                try
                {
                    return new ResultViewModel { Validate = 1, Message = "Sending AccessGroupToTerminal queued" };
                }
                catch (Exception exception)
                {
                    return new ResultViewModel { Validate = 0, Message = exception.ToString() };
                }
            });
        }
    }
}