using System.Collections.Generic;
using System.Threading.Tasks;
using Biovation.Domain;
using Biovation.Repository;
using Biovation.Repository.Sql.v1;

namespace Biovation.Service.Sql.v1
{
    public class DeviceGroupService
    {
        private readonly DeviceGroupRepository _deviceGroupRepository;

        public DeviceGroupService(DeviceGroupRepository deviceGroupRepository)
        {
            _deviceGroupRepository = deviceGroupRepository;
        }

        /// <summary>
        /// <En>Call a repository method to get all devices from database.</En>
        /// <Fa>با صدا کردن یک تابع در ریپوزیتوری اطلاعات تمامی دستگاه ها را از دیتابیس دریافت میکند.</Fa>
        /// </summary>
        /// <returns></returns>
        public List<DeviceGroup> GetAllDeviceGroup(long userId)
        {
            return _deviceGroupRepository.GetDeviceGroups(null, userId);
        }

        public List<DeviceGroup> GetDeviceGroup(int id, long userId)
        {
            return _deviceGroupRepository.GetDeviceGroups(id, userId);
        }

        public Task<ResultViewModel> ModifyDeviceGroup(DeviceGroup deviceGroup)
        {
            return Task.Run(() =>_deviceGroupRepository.ModifyDeviceGroup(deviceGroup));
        }

        public ResultViewModel ModifyDeviceGroupMember(string node, int groupId)
        {
            return _deviceGroupRepository.ModifyDeviceGroupMember(node, groupId);
        }

        public List<ResultViewModel> DeleteDeviceGroup(int[] ids)
        {
            var result = new List<ResultViewModel>();

            foreach (var id in ids)
            {
                result.Add(_deviceGroupRepository.DeleteDeviceGroup(id));
            }

            return result;
        }

        public List<DeviceGroup> GetAccessControlDeviceGroup(int id)
        {
            return _deviceGroupRepository.GetAccessControlDeviceGroup(id);
        }

        public List<DeviceGroup> GetDeviceGroupByAccessGroupId(int accessGroupId)
        {
            return _deviceGroupRepository.GetDeviceGroupsByAccessGroup(accessGroupId);
        }
    }
}
