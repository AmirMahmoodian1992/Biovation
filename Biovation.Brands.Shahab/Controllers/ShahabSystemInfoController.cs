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
        private readonly ServiceInstance _serviceInstance;

        public ShahabSystemInfoController(ServiceInstance serviceInstance)
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
    }
}
