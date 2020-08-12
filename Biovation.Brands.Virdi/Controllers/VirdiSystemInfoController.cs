using System.Linq;
using System.Reflection;
using Biovation.CommonClasses.Models;
using Microsoft.AspNetCore.Mvc;

namespace Biovation.Brands.Virdi.Controllers
{
    [Route("Biovation/Api/[controller]/[action]")]
    public class VirdiSystemInfoController : Controller
    {
        [HttpGet]
        public ResultViewModel<ModuleInfo> GetInfo()
        {
            var brandInfo = new ModuleInfo();
            var result = new ResultViewModel<ModuleInfo>();
            brandInfo.Name = Assembly.GetExecutingAssembly().GetName().Name.Split('.').LastOrDefault();
            brandInfo.Version = Assembly.GetExecutingAssembly().GetName().Version.ToString();
            result.Data = brandInfo;
            return result;
        }
    }
}