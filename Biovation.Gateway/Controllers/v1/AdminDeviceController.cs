using Biovation.CommonClasses.Models;
using Biovation.CommonClasses.Service;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;

namespace Biovation.Gateway.Controllers.v1
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

        [HttpGet]
        [Route("GetAdminDevicesByPersonId")]
        public List<AdminDeviceGroup> GetAdminDevicesByPersonId(int personId)
        {
            return _adminDeviceService.GetAdminDeviceGroupsByUserId(personId);
        }

        [HttpPost]
        [Route("ModifyAdminDevice")]
        public ResultViewModel ModifyAdminDevice([FromBody] JObject adminDevice)
        {
            try
            {
                var ss = adminDevice.ToString();
                ss = ss.Replace("]}\"", "]}");
                ss = ss.Replace("\"{", "{");
                ss = ss.Replace("\r\n", "");
                ss = ss.Replace(@"\", "");

                string node = JsonConvert.DeserializeXNode(ss, "Root")?.ToString();
                var result = _adminDeviceService.ModifyAdminDevice(node);
                return result;
            }
            catch (Exception e)
            {
                return new ResultViewModel { Message = e.Message, Validate = 0 };
            }
        }
    }
}