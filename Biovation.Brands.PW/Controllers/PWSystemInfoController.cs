using System;
using System.Linq;
using System.Reflection;
using Biovation.Domain;
using Microsoft.AspNetCore.Mvc;

namespace Biovation.Brands.PW.Controllers
{
    [ApiController]
    [Route("Biovation/Api/[controller]/[action]")]
    public class SystemInfoController : ControllerBase
    {

        [HttpGet]
        public ResultViewModel<ServiceInstance> GetInfo()
        {
            var brandInfo = new ServiceInstance();
            var result = new ResultViewModel<ServiceInstance>();
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