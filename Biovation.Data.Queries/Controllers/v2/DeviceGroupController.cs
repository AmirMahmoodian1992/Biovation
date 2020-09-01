using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Threading.Tasks;
using Biovation.Domain;
using Biovation.Repository.SQL.v2;
using DataAccessLayerCore.Repositories;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;

namespace Biovation.Data.Queries.Controllers.v2
{


    //[ApiVersion("1.0")]
    [Route("biovation/api/queries/v2/[controller]")]
    public class DeviceGroupController : Controller
    {
        private readonly DeviceGroupRepository _deviceGroupRepository;

        public DeviceGroupController(DeviceGroupRepository deviceGroupRepository)
        {
            _deviceGroupRepository = deviceGroupRepository;
        }



     
        [HttpGet]
        [Route("GetDeviceGroups")]
        public Task<ResultViewModel<PagingResult<DeviceGroup>>> GetDeviceGroups(int deviceGroupId, long userId,
            int pageNumber = default, int PageSize = default)
        {
            return Task.Run(() => _deviceGroupRepository.GetDeviceGroups(deviceGroupId, userId, pageNumber, PageSize));
        }

        [HttpGet]
        [Route("GetAccessControlDeviceGroup")]
        public Task<ResultViewModel<PagingResult<DeviceGroup>>> GetAccessControlDeviceGroup(int id,
            int pageNumber = default, int PageSize = default)
        {
            return Task.Run(() => _deviceGroupRepository.GetAccessControlDeviceGroup(id, pageNumber, PageSize));
        }


        [HttpGet]
        [Route("GetDeviceGroupsByAccessGroup")]
        public Task<ResultViewModel<PagingResult<DeviceGroup>>> GetDeviceGroupsByAccessGroup(int accessGroupId,
            int pageNumber = default, int PageSize = default)
        {
            return Task.Run(() => _deviceGroupRepository.GetDeviceGroupsByAccessGroup(accessGroupId, pageNumber, PageSize));

        }

    }
}
