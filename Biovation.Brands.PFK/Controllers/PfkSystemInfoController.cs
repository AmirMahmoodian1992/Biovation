using System;
using System.Linq;
using System.Reflection;
using System.Web.Http;
using Biovation.Domain;

namespace Biovation.Brands.PFK.Controllers
{
#if NETCORE31
    [ApiController]
    [Route("Biovation/Api/[controller]/[action]")]
    public class PfkSystemInfoController : ControllerBase
#elif NET472
    public class PfkSystemInfoController : ApiController
#endif
    {
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