using Biovation.Domain;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;
using Biovation.Repository.Sql.v2;

namespace Biovation.Data.Commands.Controllers.v2
{

    //[ApiVersion("1.0")]
    [Route("biovation/api/v2/[controller]")]
    public class DeviceGroupController : Controller
    {
        private readonly DeviceGroupRepository _deviceGroupRepository;

        public DeviceGroupController(DeviceGroupRepository deviceGroupRepository)
        {
            _deviceGroupRepository = deviceGroupRepository;
        }

        //TODO:Add DeviceGroup
        [HttpPost]
        /*public Task<ResultViewModel> AddDeviceGroup([FromBody]DeviceGroup deviceGroup = default)
         {

         }*/
        [HttpPut]
        public Task<ResultViewModel> ModifyDeviceGroup([FromBody]DeviceGroup deviceGroup)
        {
            return Task.Run(() => _deviceGroupRepository.ModifyDeviceGroup(deviceGroup));
        }


        [HttpPut]
        [Route("ModifyDeviceGroupMember")]
        public Task<ResultViewModel> ModifyDeviceGroupMember(string node, int groupId)
        {
            return Task.Run(() => _deviceGroupRepository.ModifyDeviceGroupMember(node, groupId));
        }

        [HttpDelete]
        [Route("{id}")]     
        public Task<ResultViewModel> DeleteDeviceGroup(int id)
        {
            return Task.Run(() => _deviceGroupRepository.DeleteDeviceGroup(id));
        }

        [HttpDelete]
        [Route("DeleteDeviceGroupMember/{id}")]
        public Task<ResultViewModel> DeleteDeviceGroupMember(uint id)
        {
            return Task.Run(() => _deviceGroupRepository.DeleteDeviceGroupMember(id));
        }
    }

}
