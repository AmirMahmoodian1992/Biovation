using System.Threading.Tasks;
using Biovation.Domain;
using Biovation.Service.Api.v2;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json.Linq;

namespace Biovation.Server.Controllers.v2
{
    //[Route("Biovation/Api/{controller}/{action}", Name = "Device")]
    [Route("biovation/api/v{version:apiVersion}/[controller]")]
    [ApiVersion("2.0")]
    [Authorize]
    public class AdminDeviceController : Controller
    {
        private readonly AdminDeviceService _adminDeviceService;

        public AdminDeviceController(AdminDeviceService adminDeviceService)
        {
            _adminDeviceService = adminDeviceService;
        }

        [HttpGet]
        [Route("{userId}")]
        public Task<ResultViewModel<PagingResult<AdminDeviceGroup>>> GetAdminDeviceGroupsByUserId(int userId = default, int pageNumber = default, int pageSize = default)
        {
            return Task.Run(() => _adminDeviceService.GetAdminDeviceGroupsByUserId(userId, pageNumber, pageSize));
        }

        [HttpPost]
        public Task<IActionResult> AddAdminDevice([FromBody]AdminDevice adminDevice = default)
        {
            throw null;
        }

        [HttpPut]
        public Task<ResultViewModel> ModifyAdminDevice([FromBody] JObject adminDevice = default)
        {
            var token = (string)HttpContext.Items["Token"];
            return Task.FromResult(_adminDeviceService.ModifyAdminDevice(adminDevice,token));
        }
    }
}