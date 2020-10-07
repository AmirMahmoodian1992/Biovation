using Biovation.Domain;
using Biovation.Service.Api.v1;
using Microsoft.AspNetCore.Mvc;
using System.Linq;
using System.Reflection;

namespace Biovation.Brands.ZK.ApiControllers
{
    public class ZKSystemInfoController : Controller
    {
        private readonly DeviceService _deviceService;

        public ZKSystemInfoController(DeviceService deviceService)
        {
            _deviceService = deviceService;
        }

        [HttpGet]
        public ResultViewModel<ServiceInfo> GetInfo()
        {
            var brandInfo = new ServiceInfo();
            var result = new ResultViewModel<ServiceInfo>();
            brandInfo.Name = Assembly.GetExecutingAssembly().GetName().Name.Split('.').LastOrDefault(); ;
            brandInfo.Version = Assembly.GetExecutingAssembly().GetName().Version.ToString();
            result.Data = brandInfo;
            return result;
        }
    }
}