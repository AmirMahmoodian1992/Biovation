using System.Linq;
using System.Reflection;
using System.Web.Http;
using Biovation.CommonClasses.Models;

namespace Biovation.Brands.Suprema.ApiControllers
{
    public class SupremaSystemInfoController : ApiController
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