using Biovation.Domain;
using Biovation.Repository.API.v2;

namespace Biovation.Service.API.v2
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
        //public List<DeviceBasicInfo> GetDevice(long adminUserId = default)
        //{
        //    return _deviceRepository.GetDevice(adminUserId);
        //}
        public ResultViewModel<PagingResult<DeviceBasicInfo>> GetDevices( long adminUserId = default, int deviceGroupId = default, uint Code = default, int brandId = default, string deviceName = null, int deviceModelId = default, int typeId = default, int pageNumber = default, int PageSize = default)
        {
            return _deviceRepository.GetDevices(adminUserId, deviceGroupId, Code, brandId, deviceName, deviceModelId, typeId,pageNumber,PageSize);
        }
    }
}
