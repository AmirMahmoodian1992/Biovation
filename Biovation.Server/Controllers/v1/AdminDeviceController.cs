using Biovation.CommonClasses.Manager;
using Biovation.Domain;
using Biovation.Service.Api.v1;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json.Linq;
using System.Collections.Generic;
using Newtonsoft.Json;
using JsonSerializer = System.Text.Json.JsonSerializer;

namespace Biovation.Server.Controllers.v1
{
    [ApiController]
    [ApiVersion("1.0")]
    [Route("biovation/api/v{version:apiVersion}/[controller]")]
    public class AdminDeviceController : ControllerBase
    {
        private readonly AdminDeviceService _adminDeviceService;
        private readonly string _kasraAdminToken;

        public AdminDeviceController([FromBody] AdminDeviceService adminDeviceService, BiovationConfigurationManager biovationConfigurationManager)
        {
            _adminDeviceService = adminDeviceService;
            _kasraAdminToken = biovationConfigurationManager.KasraAdminToken;
        }

        [HttpGet, Route("GetAdminDevicesByPersonId")]
        public List<AdminDeviceGroup> GetAdminDevicesByPersonId(int personId)
        {
            return _adminDeviceService.GetAdminDeviceGroupsByUserId(personId: personId, token: _kasraAdminToken);
        }

        [HttpPost, Route("ModifyAdminDevice")]
        //public ResultViewModel ModifyAdminDevice([FromBody] Dictionary<string, object> adminDevice)
        public ResultViewModel ModifyAdminDevice([FromBody] object adminDevice)
        {
            var adminDeviceSerializedData = JsonConvert.DeserializeObject<JObject>(JsonSerializer.Serialize(adminDevice));
            return _adminDeviceService.ModifyAdminDevice(adminDeviceSerializedData, token: _kasraAdminToken);
        }
    }
}
