using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Biovation.Domain;
using Biovation.Repository;

namespace Biovation.Service
{
    public class DeviceService
    {
        private readonly DeviceRepository _deviceRepository;

        public DeviceService(DeviceRepository deviceRepository)
        {
            _deviceRepository = deviceRepository;
        }
        /// <summary>
        /// <En>Call a repository method to get all devices from database.</En>
        /// <Fa>با صدا کردن یک تابع در ریپوزیتوری اطلاعات تمامی دستگاه ها را از دیتابیس دریافت میکند.</Fa>
        /// </summary>
        /// <returns></returns>
        public List<DeviceBasicInfo> GetAllDevicesBasicInfos(long adminUserId = 0)
        {
            return _deviceRepository.GetAllDevicesBasicInfos(adminUserId);
        }
        public List<DeviceBasicInfo> GetAllDevicesBasicInfosByfilter(long adminUserId = 0, int deviceGroupId = 0, uint Code = 0, int deviceId = 0, int brandId = 0, string deviceName = null, int deviceModelId = 0)
        {
            return _deviceRepository.GetAllDevicesBasicInfosByFilter(adminUserId, deviceGroupId, Code, deviceId, brandId, deviceName, deviceModelId);
        }


        public List<DeviceBasicInfo> GetDevicesByfilter(/*long id = 0,*/long adminUserId = 0, int groupId = 0, uint code = 0,
                int brandId = 0, string name = null, int modelId = 0, int typeId = 0)
        { 
            return _deviceRepository.GetDevicesByFilter(/*id,*/ adminUserId, groupId, code, brandId, name, modelId, typeId);
        }
        public ResultViewModel AddDevice(DeviceBasicInfo device)
        {
            return _deviceRepository.AddDevice(device);
        }
        /// <summary>
        /// گرفتن لیستی از دستگاه ها بر اساس فیلتر اعمال شده
        /// </summary>
        /// <param name="deviceName"></param>
        /// <param name="deviceModelId"></param>
        /// <param name="deviceTypeId"></param>
        /// <param name="userId"></param>
        /// <returns></returns>
        public List<DeviceBasicInfo> GetDevicesBasicInfosByFilter(string deviceName, int deviceModelId, int deviceTypeId, long userId)
        {
            return _deviceRepository.GetDevicesBasicInfosByFilter(deviceName, deviceModelId, deviceTypeId, userId);
        }

        /// <summary>
        /// <En>Call a repository method to get the devices info from database.</En>
        /// <Fa>با صدا کردن یک تابع در ریپوزیتوری اطلاعات یک دستگاه را از دیتابیس دریافت میکند.</Fa>
        /// </summary>
        /// <param name="deviceId">کد ساعت</param>
        /// <returns></returns>
        public DeviceBasicInfo GetDeviceInfo(int deviceId, int adminUserId = 0)
        {
            return _deviceRepository.GetDeviceBasicInfo(deviceId, adminUserId);
        }

        /// <summary>
        /// <En>Call a repository method to get the devices info from database.</En>
        /// <Fa>با صدا کردن یک تابع در ریپوزیتوری اطلاعات یک دستگاه را از دیتابیس دریافت میکند.</Fa>
        /// </summary>
        /// <param name="code">کد ساعت</param>
        /// <param name="brandId">برند</param>
        /// <returns></returns>
        public DeviceBasicInfo GetDeviceBasicInfoWithCode(uint code, string brandId, int adminUserId = 0)
        {
            return _deviceRepository.GetDeviceBasicInfoWithCode(code, brandId, adminUserId);
        }

        public List<DeviceBasicInfo> GetAllDevicesBasicInfosByBrandId(string brandId, int adminUserId = 0)
        {
            return _deviceRepository.GetAllDevicesBasicInfosByBrandId(brandId, adminUserId);
        }

        public List<DeviceBasicInfo> GetAllDevicesBasicInfosByDeviceModelId(int modelId, int adminUserId = 0)
        {
            return _deviceRepository.GetAllDevicesBasicInfosByDeviceModelId(modelId, adminUserId);
        }

        public DeviceBasicInfo GetDeviceBasicInfoByIdAndBrandId(int deviceId, string brandId, int adminUserId = 0)
        {
            return _deviceRepository.GetDeviceBasicInfoByIdAndBrandId(deviceId, brandId, adminUserId);
        }

        public DeviceBasicInfo GetDeviceBasicInfoByIdAndDeviceModelId(int deviceId, int modelId, int adminUserId = 0)
        {
            return _deviceRepository.GetDeviceBasicInfoByIdAndDeviceModelId(deviceId, modelId, adminUserId);
        }

        public List<Lookup> GetDeviceBrands()
        {
            return _deviceRepository.GetDeviceBrands();
        }

        public Lookup GetDeviceBrandById(string brandCode)
        {
            return _deviceRepository.GetDeviceBrandById(brandCode);
        }

        public List<DeviceBasicInfo> GetDevicesBasicInfosByName(string name, int userCode = 0)
        {
            return _deviceRepository.GetDevicesBasicInfosByName(name, userCode);
        }

        public DeviceModel GetDeviceModelByName(string name)
        {
            return _deviceRepository.GetDeviceModelByName(name);
        }

        public List<DeviceModel> GetDeviceModelsByBrandCode(string brandCode = default)
        {
            return _deviceRepository.GetDeviceModelsByBrandId(brandCode);
        }

        public List<DeviceModel> GetDeviceModelsByFilter(string brandCode = default, string name = default)
        {
            return _deviceRepository.GetDeviceModelsByFilter(brandCode, name);
        }


        //public ResultViewModel AddDevice(DeviceBasicInfo device)
        //{
        //    var _deviceRepository = new DeviceRepository();
        //    return _deviceRepository.AddDevice(device);
        //}

        public ResultViewModel AddDeviceModel(DeviceModel deviceModel)
        {
            return _deviceRepository.AddDeviceModel(deviceModel);
        }

        public ResultViewModel ModifyDeviceBasicInfoByID(DeviceBasicInfo device)
        {
            return _deviceRepository.ModifyDeviceBasicInfoByID(device);
        }

        public ResultViewModel DeleteDevice(uint deviceId)
        {
            return _deviceRepository.DeleteDevice(deviceId);
        }

        public AuthModeMap GetBioAuthModeWithDeviceId(int deviceId, int authMode)
        {
            return _deviceRepository.GetBioAuthModeWithDeviceId(deviceId, authMode);
        }

        public DateTime GetLastConnectedTime(uint deviceId)
        {
            return _deviceRepository.GetLastConnectedTime(deviceId);
        }

        public ResultViewModel AddDeviceConnectionTime(DeviceBasicInfo device)
        {
            return _deviceRepository.AddNetworkConnectionLog(device);
        }

        public Task<List<User>> GetAuthorizedUsersOfDevice(int deviceId)
        {
            return _deviceRepository.GetAuthorizedUsersOfDevice(deviceId);
        }
    }
}
