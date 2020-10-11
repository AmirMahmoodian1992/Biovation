using System.Linq;
using System.Reflection;
using Biovation.Domain;
using Microsoft.AspNetCore.Mvc;

namespace Biovation.Brands.EOS.Controllers
{
    public class EosSystemInfoController : Controller
    {
        [HttpGet]
        public ResultViewModel<ServiceInfo> GetInfo()
        {
            var brandInfo = new ServiceInfo();
            var result = new ResultViewModel<ServiceInfo>();
            brandInfo.Name = Assembly.GetExecutingAssembly().GetName().Name.Split('.').LastOrDefault();
            brandInfo.Version = Assembly.GetExecutingAssembly().GetName().Version.ToString();
            result.Data = brandInfo;
            return result;
        }
    }
}