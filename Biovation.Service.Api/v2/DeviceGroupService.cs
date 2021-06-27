using System.Threading.Tasks;
using Biovation.Domain;
using Biovation.Repository.Api.v2;

namespace Biovation.Service.Api.v2
{
    public class DeviceGroupService
    {
        readonly DeviceGroupRepository _deviceGroupRepository;

        public DeviceGroupService(DeviceGroupRepository deviceGroupRepository)
        {
            _deviceGroupRepository = deviceGroupRepository;
        }

        public ResultViewModel<PagingResult<DeviceGroup>> GetDeviceGroups(int deviceGroupId = default,
            int pageNumber = default, int pageSize = default, string token = default)
        {
            return _deviceGroupRepository.GetDeviceGroups(deviceGroupId, pageNumber, pageSize, token);
        }

        public async Task<ResultViewModel<PagingResult<DeviceGroup>>> GetAccessControlDeviceGroup(int id =default,
            int pageNumber = default, int pageSize = default, string token = default)
        {
            return await _deviceGroupRepository.GetAccessControlDeviceGroup(id, pageNumber, pageSize, token);
        }

        public async Task<ResultViewModel> ModifyDeviceGroup(DeviceGroup deviceGroup =default, string token = default)
        {
            return await _deviceGroupRepository.ModifyDeviceGroup(deviceGroup, token);
        }

        public async Task<ResultViewModel> ModifyDeviceGroupMember(string node = default, int groupId =default, string token = default)
        {
            return await _deviceGroupRepository.ModifyDeviceGroupMember(node, groupId, token);
        }

        public async Task<ResultViewModel> DeleteDeviceGroup(int id =default, string token = default)
        {
            return await _deviceGroupRepository.DeleteDeviceGroup(id,token);
        }

        public async Task<ResultViewModel> DeleteDeviceGroupMember(uint id = default, string token = default)
        {
            return await _deviceGroupRepository.DeleteDeviceGroupMember(id,token);
        }


    }
}
