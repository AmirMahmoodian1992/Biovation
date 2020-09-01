using Biovation.Domain;
using Biovation.Repository.SQL.v2;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;

namespace Biovation.Data.Commands.Controllers.v2
{

    //[ApiVersion("1.0")]
    [Route("biovation/api/commands/v2/[controller]")]
    public class DeviceGroupController : Controller
    {
        private readonly DeviceGroupRepository _deviceGroupRepository;

        public DeviceGroupController(DeviceGroupRepository deviceGroupRepository)
        {
            _deviceGroupRepository = deviceGroupRepository;
        }

        /// <summary>
        /// <En>Get the device info from database.</En>
        /// <Fa>اطلاعات یک گروه را از دیتابیس دریافت میکند.</Fa>
        /// </summary>
        /// <param name="deviceGroupId">کد گروه</param>
        /// <returns></returns>

        public Task<ResultViewModel> ModifyDeviceGroup(DeviceGroup deviceGroup)
        {
            return Task.Run(() => _deviceGroupRepository.ModifyDeviceGroup(deviceGroup));
        }

        public Task<ResultViewModel> ModifyDeviceGroupMember(string node, int groupId)
        {
            return Task.Run(() => _deviceGroupRepository.ModifyDeviceGroupMember(node, groupId));
        }


        public Task<ResultViewModel> DeleteDeviceGroup(int id)
        {
            return Task.Run(() => _deviceGroupRepository.DeleteDeviceGroup(id));
        }

        public Task<ResultViewModel> DeleteDeviceGroupMember(uint id)
        {
            return Task.Run(() => _deviceGroupRepository.DeleteDeviceGroupMember(id));
        }
    }

}
