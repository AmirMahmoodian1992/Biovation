using Biovation.Brands.Suprema.Devices;
using Biovation.CommonClasses;
using Biovation.CommonClasses.Interface;
using Biovation.Constants;
using Biovation.Domain;
using Biovation.Service.Api.v1;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using RestSharp;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Biovation.Brands.Suprema.Commands
{
    public class Suprema_UserAdaptation : ICommand
    {

        /// <summary>
        /// All connected devices
        /// </summary>
        private readonly DeviceService _deviceService;
        private readonly UserService _userService;


        private readonly TaskTypes _taskTypes;
        private readonly TaskService _taskService;
        private readonly TaskStatuses _taskStatuses;
        private readonly TaskItemTypes _taskItemTypes;
        private readonly TaskPriorities _taskPriorities;
        private readonly RestClient _restClient;


        private Dictionary<uint, Device> OnlineDevices { get; }
        private uint CreatorUserId { get; set; }

        private uint UserCode { get; set; }
        private uint CorrectedUserCode { get; set; }
        private uint Code { get; set; }
        private TaskItem TaskItem { get; }
        public Suprema_UserAdaptation(IReadOnlyList<object> items, Dictionary<uint, Device> devices, DeviceService deviceService, TaskTypes taskTypes, TaskService taskService,
            TaskStatuses taskStatuses, TaskItemTypes taskItemTypes, TaskPriorities taskPriorities, UserService userService, RestClient restClient)
        {
            var taskItem = (TaskItem)items[0];
            TaskItem = taskItem;

            _deviceService = deviceService;


            OnlineDevices = devices;
            _taskTypes = taskTypes;
            _taskService = taskService;
            _taskStatuses = taskStatuses;
            _taskItemTypes = taskItemTypes;
            _taskPriorities = taskPriorities;
            _userService = userService;
            _restClient = restClient;
        }

        public object Execute()
        {
            if (TaskItem is null)
                return new ResultViewModel { Id = TaskItem.Id, Code = Convert.ToInt64(TaskStatuses.FailedCode), Message = $"Error in processing task item {TaskItem.Id}.{Environment.NewLine}", Validate = 0 };

            var deviceId = TaskItem.DeviceId;
            Code = (_deviceService.GetDevices(brandId: DeviceBrands.ZkTecoCode).FirstOrDefault(d => d.DeviceId == deviceId)?.Code ?? 0);
            if (OnlineDevices.All(dev => dev.Key != Code))
            {
                Logger.Log($"The device: {Code} is not connected.");
                return new ResultViewModel { Validate = 0, Id = deviceId, Code = Convert.ToInt64(TaskStatuses.DeviceDisconnectedCode) };
            }

            var data = (JObject)JsonConvert.DeserializeObject(TaskItem.Data);
            if (data != null)
            {
                UserCode = Convert.ToUInt32(data["userCode"]);
                CorrectedUserCode = Convert.ToUInt32(data["CorrectedUserCode"]);
                CreatorUserId = Convert.ToUInt32(data["CreatorUserId"]);
            }
            var device = _deviceService.GetDevice(deviceId);
            if (device is null)
                return new ResultViewModel { Id = TaskItem.Id, Code = Convert.ToInt64(TaskStatuses.FailedCode), Message = $"Error in processing task item {TaskItem.Id}, wrong or zero device id is provided.{Environment.NewLine}", Validate = 0 };

            if (!OnlineDevices.ContainsKey(device.Code))
                return new ResultViewModel { Id = TaskItem.Id, Code = Convert.ToInt64(TaskStatuses.DeviceDisconnectedCode), Message = $"  Enroll User face from device: {device.Code} failed. The device is disconnected.{Environment.NewLine}", Validate = 0 };

            try
            {
                var creatorUser = _userService.GetUsers(userId: CreatorUserId).FirstOrDefault();
                var onlineDevice = OnlineDevices.FirstOrDefault(dev => dev.Key == Code).Value;
                var correctedUser = onlineDevice.GetUser(UserCode);



                if (correctedUser == null)
                {
                    Logger.Log($"User {UserCode}  doesn't exist on Device {device.DeviceId}");
                    return new ResultViewModel
                    {
                        Success = false,
                        Id = deviceId,
                        Code = Convert.ToInt64(TaskStatuses.FailedCode),
                        Message = $"User {UserCode}  doesn't exist on Device {device.DeviceId}"
                    };
                }

                var task = new TaskInfo
                {
                    CreatedAt = DateTimeOffset.Now,
                    CreatedBy = creatorUser,
                    TaskType = _taskTypes.DeleteUsers,
                    Priority = _taskPriorities.Medium,
                    DeviceBrand = device.Brand,
                    TaskItems = new List<TaskItem>(),
                    DueDate = DateTime.Today
                };
                task.TaskItems.Add(new TaskItem
                {
                    Status = _taskStatuses.Queued,
                    TaskItemType = _taskItemTypes.DeleteUserFromTerminal,
                    Priority = _taskPriorities.Medium,
                    DeviceId = device.DeviceId,
                    Data = JsonConvert.SerializeObject(new { userCode = UserCode }),
                    IsParallelRestricted = true,
                    IsScheduled = false,
                    OrderIndex = 1,
                    CurrentIndex = 0,
                    TotalCount = 1
                });

                _taskService.InsertTask(task);


                task = new TaskInfo
                {
                    CreatedAt = DateTimeOffset.Now,
                    CreatedBy = creatorUser,
                    TaskType = _taskTypes.SendUsers,
                    Priority = _taskPriorities.Medium,
                    DeviceBrand = device.Brand,
                    TaskItems = new List<TaskItem>(),
                    DueDate = DateTime.Today
                };

                //var correctedUser = userList.First(x => x.Code == userCode);

                correctedUser.Code = CorrectedUserCode;

                task.TaskItems.Add(new TaskItem
                {
                    Status = _taskStatuses.Queued,
                    TaskItemType = _taskItemTypes.SendUser,
                    Priority = _taskPriorities.Medium,
                    DeviceId = device.DeviceId,
                    Data = JsonConvert.SerializeObject(correctedUser),
                    IsParallelRestricted = true,
                    IsScheduled = false,
                    OrderIndex = 1,
                    CurrentIndex = 0,
                    TotalCount = 1
                });
                _taskService.InsertTask(task);

                var restRequest = new RestRequest($"{device.Brand.Name}/{device.Brand.Name}Task/RunProcessQueue", Method.POST);
                _restClient.ExecuteAsync<ResultViewModel>(restRequest);

                return new ResultViewModel
                {
                    Success = true,
                    Id = deviceId,
                    Code = Convert.ToInt64(TaskStatuses.DoneCode),
                    Message = $"The Delete and send User operations for User{UserCode}  on Device {device.DeviceId} successfully started"
                };


            }
            catch (Exception exception)
            {
                Logger.Log(exception);
                return new ResultViewModel { Validate = 0, Id = deviceId, Code = Convert.ToInt64(TaskStatuses.FailedCode) };
            }
        }

        public void Rollback()
        {
            throw new NotImplementedException();
        }

        public string GetTitle()
        {
            return "Download user photos of device.";
        }

        public string GetDescription()
        {
            return $"Download user photos of device {Code}";
        }

    }
}
