using Biovation.Domain;
using Microsoft.AspNetCore.Mvc;
using System.Linq;
using System.Reflection;

namespace Biovation.Brands.Shahab.Controllers
{
    [ApiController]
    [Route("Biovation/Api/[controller]/[action]")]
    public class ShahabSystemInfoController : ControllerBase
    {

        [HttpGet]
        public ResultViewModel<ServiceInfo> GetInfo()
        {
            var brandInfo = new ServiceInfo();
            var result = new ResultViewModel<ServiceInfo>();
            var name = Assembly.GetExecutingAssembly().GetName().Name;
            if (name != null)
                brandInfo.Name = name.Split('.').LastOrDefault();
            var version = Assembly.GetExecutingAssembly().GetName().Version;
            if (version is { })
                brandInfo.Version = version.ToString();
            result.Data = brandInfo;
            return result;
        }
    }
}
