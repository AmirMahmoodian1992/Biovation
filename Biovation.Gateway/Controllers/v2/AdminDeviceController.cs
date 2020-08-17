using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Biovation.CommonClasses.Models;
using Biovation.CommonClasses.Service;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace Biovation.Gateway.Controllers.v2
{
    //[Route("Biovation/Api/{controller}/{action}", Name = "Device")]
    [Route("biovation/api/v{version:apiVersion}/[controller]")]
    [ApiVersion("2.0")]
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

        [HttpGet]
        [Route("{id}")]
        public Task<IActionResult> GetAdminDevicesByPersonId(int id = default)
        {
            throw null;
        }

        [HttpPost]
        public Task<IActionResult> AddAdminDevice([FromBody]AdminDevice adminDevice = default)
        {
            throw null;
        }

        [HttpPut]
        public Task<IActionResult> ModifyAdminDevice([FromBody]AdminDevice adminDevice = default)
        {
            throw null;
        }
    }
}