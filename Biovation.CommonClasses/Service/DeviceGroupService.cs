using Biovation.CommonClasses.Models;
using Biovation.CommonClasses.Repository;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Biovation.CommonClasses.Service
{
    public class DeviceGroupService
    {
        private DeviceGroupRepository _deviceRepository = new DeviceGroupRepository();

        /// <summary>
        /// <En>Call a repository method to get all devices from database.</En>
        /// <Fa>با صدا کردن یک تابع در ریپوزیتوری اطلاعات تمامی دستگاه ها را از دیتابیس دریافت میکند.</Fa>
        /// </summary>
        /// <returns></returns>
        public List<DeviceGroup> GetAllDeviceGroup(long userId)
        {
            return _deviceRepository.GetDeviceGroups(null, userId);
        }

        public List<DeviceGroup> GetDeviceGroup(int id, long userId)
        {
            return _deviceRepository.GetDeviceGroups(id, userId);
        }

        public Task<ResultViewModel> ModifyDeviceGroup(DeviceGroup deviceGroup)
        {
            return Task.Run(() =>_deviceRepository.ModifyDeviceGroup(deviceGroup));
        }

        public ResultViewModel ModifyDeviceGroupMember(string node, int groupId)
        {
            return _deviceRepository.ModifyDeviceGroupMember(node, groupId);
        }

        public List<ResultViewModel> DeleteDeviceGroup(int[] ids)
        {
            var result = new List<ResultViewModel>();

            foreach (var id in ids)
            {
                result.Add(_deviceRepository.DeleteDeviceGroup(id));
            }

            return result;
        }

        public List<DeviceGroup> GetAccessControlDeviceGroup(int id)
        {
            return _deviceRepository.GetAccessControlDeviceGroup(id);
        }

        public List<DeviceGroup> GetDeviceGroupByAccessGroupId(int accessGroupId)
        {
            return _deviceRepository.GetDeviceGroupsByAccessGroup(accessGroupId);
        }
    }
}
