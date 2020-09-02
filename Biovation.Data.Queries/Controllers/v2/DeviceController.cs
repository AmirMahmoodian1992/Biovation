using Biovation.Domain;
using Biovation.Repository.SQL.v2;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

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
            uint code = 0,
            int brandId = 0, string name = null, int modelId = 0, int typeId = 0, int pageNumber = default,
            int PageSize = default)
        {
            return Task.Run(() => _deviceRepository.GetDevices(adminUserId, groupId, code, brandId, name, modelId,
                typeId, pageNumber, PageSize));
        }

        [HttpGet]
        [Route("{id?}")]
        public Task<ResultViewModel<DeviceBasicInfo>> Device([FromRoute] long id = 0, int adminUserId = 0)
        {
            return Task.Run(() => _deviceRepository.GetDevice(id, adminUserId));
        }


        [HttpGet]
        [Route("GetDeviceModels/{id}")]
        public Task<PagingResult<DeviceModel>> GetDeviceModels(long id = 0, string brandId = default,
            string name = default, int pageNumber = default, int PageSize = default)
        {
            return Task.Run(() => _deviceRepository.GetDeviceModels(id, brandId, name, pageNumber, PageSize));
        }


        [HttpGet]
        [Route("GetBioAuthModeWithDeviceId")]
        public Task<ResultViewModel<AuthModeMap>> GetBioAuthModeWithDeviceId(int deviceId, int authMode)
        {
            return Task.Run(() => _deviceRepository.GetBioAuthModeWithDeviceId(deviceId, authMode));
        }

        [HttpGet]
        [Route("GetLastConnectedTime")]
        public Task<ResultViewModel<DateTime>> GetLastConnectedTime(uint deviceId)
        {
            return Task.Run(() => _deviceRepository.GetLastConnectedTime(deviceId));
        }


        [HttpGet]
        [Route("GetDeviceBrands")]
        public Task<PagingResult<Lookup>> GetDeviceBrands(int code = default, string name = default,
            int pageNumber = default, int PageSize = default)
        {
            return Task.Run(() => _deviceRepository.GetDeviceBrands(code, name, pageNumber, PageSize));
        }
    }
}