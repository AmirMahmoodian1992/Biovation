using Biovation.Domain;
using Biovation.Repository.SQL.v2;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Threading.Tasks;

namespace Biovation.Data.Commands.Controllers.v2
{
    //[Route("Biovation/Api/{controller}/{action}", Name = "Device")]
    [Route("biovation/api/commands/v2/[controller]")]
    //[ApiVersion("2.0")]
    public class AdminDeviceController : Controller
    {
        //private readonly CommunicationManager<DeviceBasicInfo> _communicationManager = new CommunicationManager<DeviceBasicInfo>();
/*
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
                return Task.Run(() => new ResultViewModel { Message = e.Message, Validate = 0, Code = 400 });
            }
        }*/
    }
}