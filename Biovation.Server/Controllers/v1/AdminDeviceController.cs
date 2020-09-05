using System.Collections.Generic;
using Biovation.Domain;
using Biovation.Service.API.v2;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json.Linq;

namespace Biovation.Server.Controllers.v1
{
    //[Route("Biovation/Api/{controller}/{action}", Name = "Device")]
    [Route("biovation/api/v{version:apiVersion}/[controller]")]
    [ApiVersion("1.0")]
    public class AdminDeviceController : Controller
    {
        //private readonly CommunicationManager<DeviceBasicInfo> _communicationManager = new CommunicationManager<DeviceBasicInfo>();

        private readonly AdminDeviceService _adminDeviceService;

        public AdminDeviceController(AdminDeviceService adminDeviceService)
        {
            _adminDeviceService = adminDeviceService;
        }

        //public AdminDeviceController()
        //{
        //    //_communicationManager.SetServerAddress($"http://localhost:{ConfigurationManager.BiovationWebServerPort}");
        //}

        [HttpGet, Route("GetAdminDevicesByPersonId")]
        public List<AdminDeviceGroup> GetAdminDevicesByPersonId(int personId)
        {
            return _adminDeviceService.GetAdminDevicesByPersonId(personId: personId).Data.Data;
        }

        [HttpPost, Route("ModifyAdminDevice")]
        public ResultViewModel ModifyAdminDevice([FromBody] JObject adminDevice)
        {
            return _adminDeviceService.ModifyAdminDevice(adminDevice);
        }
    }
}