using System;
using System.Linq;
using System.Reflection;
using Biovation.Domain;
using Microsoft.AspNetCore.Mvc;

namespace Biovation.Brands.Suprema.Controllers
{
    [ApiController]
    [Route("Biovation/Api/[controller]/[action]")]
    public class SupremaSystemInfoController : ControllerBase
    {
        [HttpGet]
        public ResultViewModel<ServiceInstance> GetInfo()
        {
            var brandInfo = new ServiceInstance();
            var result = new ResultViewModel<ServiceInstance>();
            var name = Assembly.GetExecutingAssembly().GetName().Name;
            if (name != null)
                brandInfo.Name = name.Split('.').LastOrDefault();
            var version = Assembly.GetExecutingAssembly().GetName().Version;
            if (version != null)
                brandInfo.Version = version.ToString();
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