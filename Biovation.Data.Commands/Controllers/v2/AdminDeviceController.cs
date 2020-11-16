using Biovation.Domain;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using System;
using Biovation.Repository.Sql.v2;
using JsonSerializer = System.Text.Json.JsonSerializer;

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
        public ResultViewModel ModifyAdminDevice([FromBody] object adminDevice)
        {
            try
            {
                var adminDeviceSerializedData = JsonSerializer.Serialize(adminDevice);
                adminDeviceSerializedData = adminDeviceSerializedData.Replace("]}\"", "]}");
                adminDeviceSerializedData = adminDeviceSerializedData.Replace("\"{", "{");
                adminDeviceSerializedData = adminDeviceSerializedData.Replace("\r\n", "");
                adminDeviceSerializedData = adminDeviceSerializedData.Replace(@"\", "");

                var node = JsonConvert.DeserializeXNode(adminDeviceSerializedData, "Root")?.ToString();
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