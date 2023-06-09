﻿using Biovation.Domain;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;
using Biovation.CommonClasses.Extension;
using Biovation.Repository.Sql.v2;

namespace Biovation.Data.Queries.Controllers.v2
{
    //[ApiVersion("1.0")]
    [ApiController]
    [Route("biovation/api/v2/[controller]")]
    public class DeviceGroupController : ControllerBase
    {
        private readonly DeviceGroupRepository _deviceGroupRepository;

        public DeviceGroupController(DeviceGroupRepository deviceGroupRepository)
        {
            _deviceGroupRepository = deviceGroupRepository;
        }
        
        [HttpGet]
        [Authorize]
        [Route("GetDeviceGroups")]
        public Task<ResultViewModel<PagingResult<DeviceGroup>>> GetDeviceGroups(int deviceGroupId,
            int pageNumber = default, int pageSize = default)
        {
            return Task.Run(() => _deviceGroupRepository.GetDeviceGroups(deviceGroupId, HttpContext.GetUser().Id, pageNumber, pageSize));
        }

        [HttpGet]
        [Authorize]
        [Route("GetAccessControlDeviceGroup")]
        public Task<ResultViewModel<PagingResult<DeviceGroup>>> GetAccessControlDeviceGroup(int id,
            int pageNumber = default, int pageSize = default)
        {
            return Task.Run(() => _deviceGroupRepository.GetAccessControlDeviceGroup(id, pageNumber, pageSize));
        }


        [HttpGet]
        [Route("GetDeviceGroupsByAccessGroup")]
        [Authorize]

        public Task<ResultViewModel<PagingResult<DeviceGroup>>> GetDeviceGroupsByAccessGroup(int accessGroupId,
            int pageNumber = default, int pageSize = default)
        {
            return Task.Run(() => _deviceGroupRepository.GetDeviceGroupsByAccessGroup(accessGroupId, pageNumber, pageSize));

        }

    }
}
