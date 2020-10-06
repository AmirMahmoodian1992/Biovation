using Biovation.Domain;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Threading.Tasks;
using Biovation.Repository.Sql.v2;

namespace Biovation.Data.Queries.Controllers.v2
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

        [HttpGet]
        public Task<ResultViewModel<PagingResult<DeviceBasicInfo>>> Devices(long adminUserId = 0, int groupId = 0,
            uint code = 0, int brandId = 0, string name = null, int modelId = 0, int deviceIoTypeId = 0, int pageNumber = default,
            int pageSize = default)
        {
            return Task.Run(() => _deviceRepository.GetDevices(adminUserId, groupId, code, brandId, name, modelId,
                deviceIoTypeId, pageNumber, pageSize));
        }

        [HttpGet]
        [Route("{id}")]
        public Task<ResultViewModel<DeviceBasicInfo>> Device([FromRoute] long id = 0, int adminUserId = 0)
        {
            return Task.Run(() => _deviceRepository.GetDevice(id, adminUserId));
        }

        [HttpGet]
        [Route("DeviceModels/{id?}")]
        public Task<ResultViewModel<PagingResult<DeviceModel>>> GetDeviceModels(long id = 0, string brandId = default,
            string name = default, int pageNumber = default, int pageSize = default)
        {
            return Task.Run(() => _deviceRepository.GetDeviceModels(id, brandId, name, pageNumber, pageSize));
        }

        [HttpGet]
        [Route("BioAuthModeWithDeviceId")]
        public Task<ResultViewModel<AuthModeMap>> GetBioAuthModeWithDeviceId(int deviceId, int authMode)
        {
            return Task.Run(() => _deviceRepository.GetBioAuthModeWithDeviceId(deviceId, authMode));
        }

        [HttpGet]
        [Route("LastConnectedTime")]
        public Task<ResultViewModel<DateTime>> GetLastConnectedTime(uint deviceId)
        {
            return Task.Run(() => _deviceRepository.GetLastConnectedTime(deviceId));
        }

        [HttpGet]
        [Route("DeviceBrands")]
        public Task<ResultViewModel<PagingResult<Lookup>>> GetDeviceBrands(int code = default, string name = default,
            int pageNumber = default, int pageSize = default)
        {
            return Task.Run(() => _deviceRepository.GetDeviceBrands(code, name, pageNumber, pageSize));
        }

        [HttpGet]
        [Route("GetAuthorizedUsersOfDevice")]
        public Task<ResultViewModel<PagingResult<User>>> GetAuthorizedUsersOfDevice(int deviceId)
        {
            return Task.Run(() => _deviceRepository.GetAuthorizedUsersOfDevice(deviceId));
        }
    }
}