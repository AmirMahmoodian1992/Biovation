using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Biovation.CommonClasses.Manager;
using Biovation.CommonClasses.Models;
using Biovation.CommonClasses.Service;
using Microsoft.AspNetCore.Mvc;
using MoreLinq;
using Newtonsoft.Json;
using RestSharp;

namespace Biovation.Gateway.Controllers.v2
{
    [Route("biovation/api/[controller]")]
    [ApiController]
    public class DeviceGroupController : Controller
    {
        private readonly DeviceService _deviceService;
        private readonly DeviceGroupService _deviceGroupService;

        private readonly RestClient _restClient;

        public DeviceGroupController(DeviceService deviceService, DeviceGroupService deviceGroupService)
        {
            _deviceService = deviceService;
            _deviceGroupService = deviceGroupService;
            _restClient = (RestClient)new RestClient($"http://localhost:{BiovationConfigurationManager.BiovationWebServerPort}/Biovation/Api/").UseSerializer(() => new RestRequestJsonSerializer());
        }

        [HttpGet]
        public Task<IActionResult> GetDeviceGroup(int id = default, long userId = default)
        {
            throw null;
        }

        [HttpPut]
        public Task<IActionResult> ModifyDeviceGroup(DeviceGroup deviceGroup = default)
        {
            throw null;
        }

        [HttpDelete]
        public Task<IActionResult> DeleteDeviceGroup([FromBody] int[] ids = default)
        {
            throw null;
        }


        [HttpGet]
        [Route("AccessControlDeviceGroup")]
        public Task<IActionResult> GetAccessControlDeviceGroup(int id =default)
        {
            throw null;
        }
    }
}
