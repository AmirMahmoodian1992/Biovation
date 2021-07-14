using Biovation.Constants;
using Biovation.Domain;
using Biovation.Repository.Api.v2;
using Newtonsoft.Json;
using RestSharp;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Biovation.Service.Api.v2
{
    public class DeviceService
    {
        private readonly DeviceRepository _deviceRepository;
        private readonly TaskTypes _taskTypes;
        private readonly TaskService _taskService;
        private readonly TaskStatuses _taskStatuses;
        private readonly TaskItemTypes _taskItemTypes;
        private readonly TaskPriorities _taskPriorities;


        public DeviceService(DeviceRepository deviceRepository, TaskTypes taskTypes, TaskService taskService, TaskStatuses taskStatuses, TaskItemTypes taskItemTypes, TaskPriorities taskPriorities)
        {
            _deviceRepository = deviceRepository;
            _taskTypes = taskTypes;
            _taskService = taskService;
            _taskStatuses = taskStatuses;
            _taskItemTypes = taskItemTypes;
            _taskPriorities = taskPriorities;
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
        public async Task<ResultViewModel<PagingResult<DeviceBasicInfo>>> GetDevices(int deviceGroupId = default, uint code = default, string brandId = default, string deviceName = null,
            int deviceModelId = default, int deviceIoTypeId = default, int pageNumber = default, int pageSize = default, string token = default)
        {
            return await _deviceRepository.GetDevices(deviceGroupId, code, brandId, deviceName, deviceModelId,
                deviceIoTypeId, pageNumber, pageSize, token);
        }

        public async Task<ResultViewModel<DeviceBasicInfo>> GetDevice(long id = default, string token = default)
        {
            return await _deviceRepository.GetDevice(id, token);
        }

        public async Task<ResultViewModel<PagingResult<DeviceModel>>> GetDeviceModels(long id = default, int brandId = default,
            string name = null, int pageNumber = default, int pageSize = default, string token = default)
        {
            return await _deviceRepository.GetDeviceModels(id, brandId.ToString(), name, pageNumber, pageSize, token);
        }

        public async Task<ResultViewModel<AuthModeMap>> GetBioAuthModeWithDeviceId(int id, int authMode, string token)
        {
            return await _deviceRepository.GetBioAuthModeWithDeviceId(id, authMode, token);
        }

        public async Task<ResultViewModel<DateTime>> GetLastConnectedTime(int id, string token)
        {
            return await _deviceRepository.GetLastConnectedTime((uint)id, token);
        }

        public async Task<ResultViewModel<PagingResult<Lookup>>> GetDeviceBrands(int code = default, string name = default,
            int pageNumber = default, int pageSize = default, string token = default)

        {
            return await _deviceRepository.GetDeviceBrands(code, name, pageNumber, pageSize, token);
        }

        // TODO - Verify the method
        public ResultViewModel<List<User>> RetrieveUsersOfDevice(DeviceBasicInfo device, List<User> users, User creatorUser, bool embedTemplate = false, string token = default)
        {
            var usersResult = new ResultViewModel<List<User>>();
            try
            {
                if (device is not null)
                {
                    var task = new TaskInfo
                    {
                        CreatedAt = DateTimeOffset.Now,
                        CreatedBy = creatorUser,
                        DeviceBrand = device.Brand,
                        TaskType = _taskTypes.RetrieveAllUsersFromTerminal,
                        Priority = _taskPriorities.Immediate,
                        TaskItems = new List<TaskItem>()
                    };

                    task.TaskItems.Add(new TaskItem
                    {
                        Status = _taskStatuses.Queued,
                        TaskItemType = _taskItemTypes.RetrieveAllUsersFromTerminal,
                        Priority = _taskPriorities.Immediate,
                        DeviceId = device.DeviceId,
                        Data = JsonConvert.SerializeObject(new { device.DeviceId, embedTemplate }),
                        IsParallelRestricted = true,
                        IsScheduled = false,
                        OrderIndex = 1,

                    });

                    _taskService.InsertTask(task);
                    usersResult =  _deviceRepository.RetrieveUsersOfDevice(device, task, token);
                }
            }
            catch (Exception)
            {
                //ignore
            }
            var joinResult = (from r in usersResult?.Data
                              join u in users on r.Code equals u.Code
                                  into ps
                              from u in ps.DefaultIfEmpty()
                              select new User
                              {
                                  Type = u == null ? 0 : 1,
                                  IsActive = r.IsActive,
                                  Id = r.Id,
                                  Code = r.Code,
                                  FullName = u != null ? u.FirstName + " " + u.SurName : r.UserName,
                                  StartDate = u?.StartDate ?? new DateTime(1990, 1, 1),
                                  EndDate = u?.EndDate ?? new DateTime(2050, 1, 1)
                              }).ToList();

            var lastResult = new ResultViewModel<List<User>>
            {
                Data = joinResult
            };
            return lastResult;
        }

        // TODO - Verify method.
        public IRestResponse<ResultViewModel> ReadOfflineOfDevice(DeviceBasicInfo device, DateTime? fromDate, DateTime? toDate, string token = default)
        {
            return _deviceRepository.ReadOfflineOfDevice(device, fromDate, toDate, token);
        }

        public ResultViewModel RemoveUserFromDevice(DeviceBasicInfo device, string token = default)
        {
            return _deviceRepository.RemoveUserFromDevice(device, token);
        }

        // TODO - Verify the method.
        public ResultViewModel DeleteUserFromDevice(DeviceBasicInfo device, List<long> userIds, string token = default)
        {
            return _deviceRepository.DeleteUserFromDevice(device, userIds, token);
        }

        // TODO - Verify method.
        public ResultViewModel RemoveUserFromDeviceById(DeviceBasicInfo device, long userId, string token = default)
        {
            return _deviceRepository.RemoveUserFromDeviceById(device, userId, token);
        }

        // TODO - Verify the method.
        public async Task<List<DeviceBasicInfo>> GetOnlineDevices(string token = default)
        {
            return await _deviceRepository.GetOnlineDevices(token);
        }

        // TODO - Verify the method.
        public IRestResponse<ResultViewModel> ClearLogOfDevice(DeviceBasicInfo device, DateTime? fromDate, DateTime? toDate, string token)
        {
            return _deviceRepository.ClearLogsOfDevice(device, fromDate, toDate, token);
        }

        // TODO - Verify the method.
        public IRestResponse<List<ResultViewModel>> RetrieveUsers(DeviceBasicInfo device, List<uint> userId = default, string token = default)
        {
            return _deviceRepository.RetrieveUsers(device, userId, token);
        }

        public async Task<ResultViewModel> AddDevice(DeviceBasicInfo device = default, string token = default)
        {
            return await _deviceRepository.AddDevice(device, token);
        }

        public async Task<ResultViewModel> AddDeviceModel(DeviceModel deviceModel = default, string token = default)
        {
            return await _deviceRepository.AddDeviceModel(deviceModel, token);
        }

        public async Task<ResultViewModel> DeleteDevice(uint id, string token = default)
        {
            return await _deviceRepository.DeleteDevice(id, token);
        }

        public async Task<ResultViewModel> DeleteDevices(List<uint> ids, string token = default)
        {
            return await _deviceRepository.DeleteDevices(ids, token);
        }

        public async Task<ResultViewModel> ModifyDevice(DeviceBasicInfo device, string token = default)
        {
            return await _deviceRepository.ModifyDevice(device, token);
        }

        // TODO - Verify method.
        public void ModifyDeviceInfo(DeviceBasicInfo device, string token = default)
        {
            _deviceRepository.ModifyDeviceInfo(device, token);
        }

        public async Task<ResultViewModel> AddNetworkConnectionLog(DeviceBasicInfo device, string token = default)
        {
            return await _deviceRepository.AddNetworkConnectionLog(device, token);
        }

        public async Task<ResultViewModel<PagingResult<User>>> GetAuthorizedUsersOfDevice(int id, string token = default)
        {
            return await _deviceRepository.GetAuthorizedUsersOfDevice(id, token);
        }

        // TODO - Verify the method.
        public Task<IRestResponse<List<ResultViewModel>>> SendUserToDevice(DeviceBasicInfo device, List<long> userIds, string token = default)
        {
            return _deviceRepository.SendUserToDevice(device, userIds, token);
        }

        // TODO - Verify the method.
        public Task<IRestResponse<ResultViewModel>> SendUsersOfDevice(DeviceBasicInfo device, string token = default)
        {
            return _deviceRepository.SendUsersOfDevice(device, token);
        }

        // TODO - Verify the method.
        public Task<IRestResponse<Dictionary<string, string>>> GetAdditionalData(DeviceBasicInfo device, User creatorUser, string token = default)
        {

            var task = new TaskInfo
            {
                CreatedAt = DateTimeOffset.Now,
                CreatedBy = creatorUser,
                TaskType = _taskTypes.GetLogsInPeriod,
                Priority = _taskPriorities.Immediate,
                DeviceBrand = device.Brand,
                TaskItems = new List<TaskItem>(),
                DueDate = DateTime.Today
            };

            if (device is null)
            {
                return null;
            }

            var deviceId = device.DeviceId;
            task.TaskItems.Add(new TaskItem
            {
                Status = _taskStatuses.Done,
                TaskItemType = _taskItemTypes.GetLogsInPeriod,
                Priority = _taskPriorities.Immediate,
                DeviceId = deviceId,
                Data = JsonConvert.SerializeObject(new { deviceId }),
                IsParallelRestricted = true,
                IsScheduled = false,
                OrderIndex = 1,
                CurrentIndex = 0
            });
            _ = _taskService.InsertTask(task).Result;
            return _deviceRepository.GetAdditionalData(device, task, token);
        }
    }
}
