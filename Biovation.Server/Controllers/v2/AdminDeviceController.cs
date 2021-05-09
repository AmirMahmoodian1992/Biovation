using System;
using System.Threading.Tasks;
using Biovation.Domain;
using Biovation.Server.Attribute;
using Biovation.Service.Api.v2;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using JsonSerializer = System.Text.Json.JsonSerializer;

namespace Biovation.Server.Controllers.v2
{
    [Authorize]
    [ApiController]
    [ApiVersion("2.0")]
    [Route("biovation/api/v{version:apiVersion}/[controller]")]
    public class AdminDeviceController : ControllerBase
    {
        private readonly AdminDeviceService _adminDeviceService;

        public AdminDeviceController(AdminDeviceService adminDeviceService)
        {
            _adminDeviceService = adminDeviceService;
        }

        [HttpGet]
        [Route("{userId}")]
        public async Task<ResultViewModel<PagingResult<AdminDeviceGroup>>> GetAdminDeviceGroupsByUserId([FromRoute] int userId = default, int pageNumber = default, int pageSize = default)
        {
            return await _adminDeviceService.GetAdminDeviceGroupsByUserId(userId, pageNumber, pageSize);
        }

        [HttpPost]
        public Task<IActionResult> AddAdminDevice([FromBody] AdminDevice adminDevice = default)
        {
            throw new NotImplementedException();
        }

        [HttpPut]
        public async Task<ResultViewModel> ModifyAdminDevice([FromBody] object adminDevice = default)
        {
            var adminDeviceSerializedData = JsonConvert.DeserializeObject<JObject>(JsonSerializer.Serialize(adminDevice));
            return await _adminDeviceService.ModifyAdminDevice(adminDeviceSerializedData, HttpContext.Items["Token"] as string);
        }
    }
}