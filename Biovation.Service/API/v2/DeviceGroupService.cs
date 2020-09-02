using Biovation.Domain;
using Biovation.Repository.API.v2;

namespace Biovation.Service.API.v2
{
    public class DeviceGroupService
    {
        DeviceGroupRepository _deviceGroupRepository;

        public DeviceGroupService(DeviceGroupRepository deviceGroupRepository)
        {
            _deviceGroupRepository = deviceGroupRepository;
        }

        public ResultViewModel<PagingResult<DeviceGroup>> GetDeviceGroups(int? deviceGroupId, long userId,
            int pageNumber = default, int PageSize = default)
        {
            return _deviceGroupRepository.GetDeviceGroups(deviceGroupId, userId, pageNumber, PageSize);
        }

        public ResultViewModel<PagingResult<DeviceGroup>> GetAccessControlDeviceGroup(int id,
            int pageNumber = default, int PageSize = default)
        {
            return _deviceGroupRepository.GetAccessControlDeviceGroup(id, pageNumber, PageSize);
        }


    }
}
