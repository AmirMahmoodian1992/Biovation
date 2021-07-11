using Biovation.Domain;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Threading.Tasks;
using Biovation.CommonClasses.Extension;
using Biovation.Repository.Sql.v2;

namespace Biovation.Data.Queries.Controllers.v2
{
    //[Route("Biovation/Api/{controller}/{action}", Name = "Device")]
    //[Route("biovation/api/v{version:apiVersion}/[controller]")]
    [ApiController]
    [Route("biovation/api/v2/[controller]")]
    //[ApiVersion("2.0")]
    public class DeviceController : ControllerBase
    {
        private readonly DeviceRepository _deviceRepository;
        public DeviceController(DeviceRepository deviceRepository)
        {
            _deviceRepository = deviceRepository;
        }

        [HttpGet]
        [Authorize]
        public Task<ResultViewModel<PagingResult<DeviceBasicInfo>>> Devices( int groupId = 0,
            uint code = 0, string brandId = default, string name = null, int modelId = 0, int deviceIoTypeId = 0, int pageNumber = default,
            int pageSize = default)
        {
            return Task.Run(() => _deviceRepository.GetDevices(HttpContext.GetUser().Id, groupId, code, brandId, name, modelId,
                deviceIoTypeId, pageNumber, pageSize));
        }

        [HttpGet]
        [Route("{id}")]
        [Authorize]
        public Task<ResultViewModel<DeviceBasicInfo>> Device([FromRoute] long id = 0)
        {
            return Task.Run(() => _deviceRepository.GetDevice(id, (int)HttpContext.GetUser().Id));
        }

        [HttpGet]
        [Route("DeviceModels/{id?}")]
        [Authorize]
        public Task<ResultViewModel<PagingResult<DeviceModel>>> GetDeviceModels([FromRoute] long id = 0, string brandId = default,
            string name = default, int pageNumber = default, int pageSize = default)
        {
            return Task.Run(() => _deviceRepository.GetDeviceModels(id, brandId, name, pageNumber, pageSize));
        }

        [HttpGet]
        [Route("BioAuthModeWithDeviceId")]
        [Authorize]
        public Task<ResultViewModel<AuthModeMap>> GetBioAuthModeWithDeviceId(int deviceId, int authMode)
        {
            return Task.Run(() => _deviceRepository.GetBioAuthModeWithDeviceId(deviceId, authMode));
        }

        [HttpGet]
        [Route("LastConnectedTime")]
        [Authorize]
        public Task<ResultViewModel<DateTime>> GetLastConnectedTime(uint deviceId)
        {
            return Task.Run(() => _deviceRepository.GetLastConnectedTime(deviceId));
        }

        [HttpGet]
        [Route("DeviceBrands")]
        [Authorize]
        public Task<ResultViewModel<PagingResult<Lookup>>> GetDeviceBrands(int code = default, string name = default,
            int pageNumber = default, int pageSize = default)
        {
            return Task.Run(() => _deviceRepository.GetDeviceBrands(code, name, pageNumber, pageSize));
        }

        [HttpGet]
        [Route("GetAuthorizedUsersOfDevice")]
        [Authorize]
        public Task<ResultViewModel<PagingResult<User>>> GetAuthorizedUsersOfDevice(int deviceId)
        {
            return Task.Run(() => _deviceRepository.GetAuthorizedUsersOfDevice(deviceId));
        }
    }
}