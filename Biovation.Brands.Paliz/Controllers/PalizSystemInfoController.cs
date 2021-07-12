using System;
using System.Linq;
using System.Reflection;
using Biovation.Domain;
using Microsoft.AspNetCore.Mvc;

namespace Biovation.Brands.Paliz.Controllers
{
    [ApiController]
    [Route("Biovation/Api/[controller]/[action]")]
    public class SystemInfoController : ControllerBase
    {
        private readonly ServiceInstance _serviceInstance;

        public SystemInfoController(ServiceInstance serviceInstance)
        {
            _serviceInstance = serviceInstance;
        }

        [HttpGet]
        public ResultViewModel<ServiceInstance> GetInfo()
        {
            var result = new ResultViewModel<ServiceInstance>()
            {
                Success = true,
                Data = _serviceInstance
            };
            return result;
        }


        [HttpGet]
        public void StopService()
        {
            Environment.Exit(0);
        }
    }
}
