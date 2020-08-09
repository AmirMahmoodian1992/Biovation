using System;
using System.Collections.Generic;
using System.Web.Http;
using Biovation.CommonClasses;
using Biovation.CommonClasses.Manager;
using Biovation.CommonClasses.Models;
using Biovation.CommonClasses.Service;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace Biovation.WebService.APIControllers
{
    //[Route("Biovation/Api/{controller}/{action}", Name = "Device")]
    public class AdminDeviceController : ApiController
    {
        private readonly CommunicationManager<DeviceBasicInfo> _communicationManager = new CommunicationManager<DeviceBasicInfo>();

        private readonly AdminDeviceService _adminDeviceService = new AdminDeviceService();

        public AdminDeviceController()
        {
            _communicationManager.SetServerAddress($"http://localhost:{ConfigurationManager.BiovationWebServerPort}");
        }

        [HttpGet]
        public List<AdminDeviceGroup> GetAdminDevicesByPersonId(int personId)
        {
            return _adminDeviceService.GetAdminDeviceGroupsByUserId(personId);
        }

        [HttpPost]
        public ResultViewModel ModifyAdminDevice([FromBody] JObject adminDevice)
        {
            try
            {
                var ss = adminDevice.ToString();
                ss = ss.Replace("]}\"", "]}");
                ss = ss.Replace("\"{", "{");
                ss = ss.Replace("\r\n", "");
                ss = ss.Replace(@"\", "");

                string node = JsonConvert.DeserializeXNode(ss, "Root").ToString();
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
