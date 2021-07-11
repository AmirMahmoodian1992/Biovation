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
        public ResultViewModel<ServiceInfo> GetInfo()
        {
            var brandInfo = new ServiceInfo();
            var result = new ResultViewModel<ServiceInfo>();
            brandInfo.Name = Assembly.GetExecutingAssembly().GetName().Name?.Split('.').LastOrDefault();
            brandInfo.Version = Assembly.GetExecutingAssembly().GetName().Version?.ToString();
            result.Data = brandInfo;
            return result;
        }


        [HttpGet]
        public void StopService()
        {
            Environment.Exit(0);
        }
    }
}
