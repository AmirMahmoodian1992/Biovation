using System;
using System.Threading.Tasks;
using Biovation.Domain;
using Biovation.Repository.v2;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace Biovation.Server.Controllers.v2
{
    //[Route("Biovation/Api/{controller}/{action}", Name = "Device")]
    [Route("biovation/api/v{version:apiVersion}/[controller]")]
    //[ApiVersion("2.0")]
    public class AdminDeviceController : Controller
    {
        //private readonly CommunicationManager<DeviceBasicInfo> _communicationManager = new CommunicationManager<DeviceBasicInfo>();

        private readonly AdminDeviceRepository _adminDeviceRepository;

        [HttpPost]
        [Route("ModifyAdminDevice")]
        public Task<ResultViewModel> ModifyAdminDevice([FromBody] JObject adminDevice)
        {

            try
            {
                var ss = adminDevice.ToString();
                ss = ss.Replace("]}\"", "]}");
                ss = ss.Replace("\"{", "{");
                ss = ss.Replace("\r\n", "");
                ss = ss.Replace(@"\", "");

                string node = JsonConvert.DeserializeXNode(ss, "Root")?.ToString();
                return Task.Run(() => _adminDeviceRepository.ModifyAdminDevice(node));
               
            }
            catch (Exception e)
            {
                return Task.Run(() => new ResultViewModel { Message = e.Message, Validate = 0 ,Code = 400});
            }          
        }
    }
}