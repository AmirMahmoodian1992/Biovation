using Biovation.Domain;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.Threading.Tasks;
using Biovation.Repository.Sql.v2;


namespace Biovation.Data.Commands.Controllers.v2
{
    //[Route("Biovation/Api/{controller}/{action}", Name = "Device")]
    //[Route("biovation/api/v{version:apiVersion}/[controller]")]
    [Route("biovation/api/v2/[controller]")]
    //[ApiVersion("1.0")]
    public class DeviceController : Controller
    {
        private readonly DeviceRepository _deviceRepository;


        public DeviceController(DeviceRepository deviceRepository)
        {
            _deviceRepository = deviceRepository;
        }


        [HttpPost]
        public Task<ResultViewModel> AddDevice([FromBody] DeviceBasicInfo device = default)
        {
            return Task.Run(() => _deviceRepository.AddDevice(device));
        }
      
        [HttpPost]
        [Authorize]
        [Route("DeviceModel")]

        public Task<ResultViewModel> AddDeviceModel([FromBody]DeviceModel deviceModel)
        {
            return Task.Run(() => _deviceRepository.AddDeviceModel(deviceModel));
        }

        [HttpDelete]
        [Route("{id}")]
        [Authorize]
        public Task<ResultViewModel> DeleteDevice(uint id)
        {
            return Task.Run(() => _deviceRepository.DeleteDevice(id));
        }

        [HttpPost]
        [Route("DeleteDevices")]
        [Authorize]
        public Task<ResultViewModel> DeleteDevices([FromBody] List<uint> ids = default)
        {
            return Task.Run(() => _deviceRepository.DeleteDevices(ids));
        }


        [HttpPut]
        [Authorize]
        public Task<ResultViewModel> ModifyDevice([FromBody] DeviceBasicInfo device = default)
        {
            return Task.Run(() => _deviceRepository.ModifyDeviceBasicInfo(device));
        }     

        [HttpPost]
        [Route("NetworkConnectionLog")]
        [Authorize]
        public Task<ResultViewModel> AddNetworkConnectionLog([FromBody]DeviceBasicInfo device)
        {
            return Task.Run(() => _deviceRepository.AddNetworkConnectionLog(device));
        }
   
    }
}