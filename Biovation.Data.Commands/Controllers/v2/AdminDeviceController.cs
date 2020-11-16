using Biovation.Domain;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using Biovation.Repository.Sql.v2;

namespace Biovation.Data.Commands.Controllers.v2
{
    [ApiController]
    [Route("biovation/api/v2/[controller]")]
    public class AdminDeviceController : ControllerBase
    {
        private readonly AdminDeviceRepository _adminDeviceRepository;

        public AdminDeviceController(AdminDeviceRepository adminDeviceRepository)
        {
            _adminDeviceRepository = adminDeviceRepository;
        }
        
        [HttpPut]
        [Authorize]
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
                var result = _adminDeviceRepository.ModifyAdminDevice(node);
                return result;
            }
            catch (Exception e)
            {
                return new ResultViewModel { Message = e.Message, Validate = 0 };
            }
        }

    }
}