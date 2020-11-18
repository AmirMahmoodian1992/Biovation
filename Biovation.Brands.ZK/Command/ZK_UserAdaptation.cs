using Biovation.Brands.ZK.Devices;
using Biovation.CommonClasses;
using Biovation.CommonClasses.Interface;
using System;
using System.Collections.Generic;
using System.Linq;
using Biovation.Constants;
using Biovation.Domain;
using Biovation.Service.Api.v1;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using RestSharp;
using TaskService = Biovation.Service.Api.v2.TaskService;
using UserService = Biovation.Service.Api.v2.UserService;

namespace Biovation.Brands.ZK.Command
{
    public class ZKUserAdaptation : ICommand
    {
        /// <summary>
        /// All connected devices
        /// </summary>
        private readonly Biovation.Service.Api.v2.DeviceService _deviceService;
        private readonly Biovation.Service.Api.v2.UserService _userService;

        private TaskItem TaskItem { get; }
        private readonly TaskTypes _taskTypes;
        private readonly TaskService _taskService;
        private readonly TaskStatuses _taskStatuses;
        private readonly TaskItemTypes _taskItemTypes;
        private readonly TaskPriorities _taskPriorities;
        private readonly CommandFactory _commandFactory;

        private Dictionary<uint, Device> OnlineDevices { get; }
        private int DeviceId { get; }
        private uint Code { get; }
        private uint CreatorUserId { get; }

        private uint UserCode { get; }
        private uint CorrectedUserCode { get; }

        public ZKUserAdaptation(IReadOnlyList<object> items, Dictionary<uint, Device> devices, DeviceService deviceService, TaskItem taskItem, Biovation.Service.Api.v2.DeviceService deviceService1, TaskTypes taskTypes, TaskService taskService, TaskStatuses taskStatuses, TaskItemTypes taskItemTypes, TaskPriorities taskPriorities, UserService userService, CommandFactory commandFactory)
        {
            TaskItem = taskItem;
            DeviceId = taskItem.DeviceId;
            Code = (deviceService.GetDevices(brandId: DeviceBrands.ZkTecoCode).FirstOrDefault(d => d.DeviceId == DeviceId)?.Code ?? 0);

            var data = (JObject)JsonConvert.DeserializeObject(taskItem.Data);
            if (data != null)
            {
                UserCode = Convert.ToUInt32(data["userId"]);
                CorrectedUserCode = Convert.ToUInt32(data["saving"]);
                CreatorUserId = Convert.ToUInt32(data["creatorUser"]);
            }

            OnlineDevices = devices;
            _deviceService = deviceService1;
            _taskTypes = taskTypes;
            _taskService = taskService;
            _taskStatuses = taskStatuses;
            _taskItemTypes = taskItemTypes;
            _taskPriorities = taskPriorities;
            _userService = userService;
            _commandFactory = commandFactory;
            DeviceId = devices.FirstOrDefault(dev => dev.Key == Code).Value.GetDeviceInfo().DeviceId;
        }

        public object Execute()
        {
            if (OnlineDevices.All(device => device.Key != Code))
            {
                Logger.Log($"The device: {Code} is not connected.");
                return new ResultViewModel { Validate = 0, Id = DeviceId, Code = Convert.ToInt64(TaskStatuses.DeviceDisconnectedCode) };
            }
            if (TaskItem is null)
                return new ResultViewModel { Id = TaskItem.Id, Code = Convert.ToInt64(TaskStatuses.FailedCode), Message = $"Error in processing task item {TaskItem.Id}.{Environment.NewLine}", Validate = 0 };

            var deviceId = TaskItem.DeviceId;

            var device = _deviceService.GetDevice(deviceId)?.Data;
            if (device is null)
                return new ResultViewModel { Id = TaskItem.Id, Code = Convert.ToInt64(TaskStatuses.FailedCode), Message = $"Error in processing task item {TaskItem.Id}, wrong or zero device id is provided.{Environment.NewLine}", Validate = 0 };

            if (!OnlineDevices.ContainsKey(device.Code))
                return new ResultViewModel { Id = TaskItem.Id, Code = Convert.ToInt64(TaskStatuses.DeviceDisconnectedCode), Message = $"  Enroll User face from device: {device.Code} failed. The device is disconnected.{Environment.NewLine}", Validate = 0 };

            try
            {
                var creatorUser = _userService.GetUsers(userId: CreatorUserId).Data.Data.FirstOrDefault();

                var task = new TaskInfo
                {
                    CreatedAt = DateTimeOffset.Now,
                    CreatedBy = creatorUser,
                    DeviceBrand = device.Brand,
                    TaskType = _taskTypes.RetrieveUserFromTerminal,
                    Priority = _taskPriorities.Medium,
                    TaskItems = new List<TaskItem>()
                };
                task.TaskItems.Add(new TaskItem
                {
                    Status = _taskStatuses.Queued,
                    TaskItemType = _taskItemTypes.RetrieveUserFromTerminal,
                    Priority = _taskPriorities.Medium,

                    DeviceId = device.DeviceId,
                    Data = JsonConvert.SerializeObject(new { UserCode, saving = false }),
                    IsParallelRestricted = true,
                    IsScheduled = false,
                    OrderIndex = 1,

                });

                var correctedUser = (ResultViewModel<User>)_commandFactory.Factory(CommandType.RetrieveUserFromDevice,
                    new List<object> { task.TaskItems.FirstOrDefault() }).Execute();


                if (correctedUser == null)
                {
                    Logger.Log($"User {UserCode}  doesn't exist on Device {device.DeviceId}");
                    return new ResultViewModel
                    {
                        Success = false,
                        Id = DeviceId,
                        Code = Convert.ToInt64(TaskStatuses.FailedCode),
                        Message = $"User {UserCode}  doesn't exist on Device {device.DeviceId}"
                    };
                }



                task = new TaskInfo
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
                    Data = JsonConvert.SerializeObject(new { UserCode }),
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

                return new ResultViewModel
                {
                    Success = true,
                    Id = DeviceId,
                    Code = Convert.ToInt64(TaskStatuses.DoneCode),
                    Message = $"The Delete and send User operations for User{UserCode}  on Device {device.DeviceId} successfully started"
                };


            }
            catch (Exception exception)
            {
                Logger.Log(exception);
                return new ResultViewModel { Validate = 0, Id = DeviceId, Code = Convert.ToInt64(TaskStatuses.FailedCode) };
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
