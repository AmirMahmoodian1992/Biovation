using Biovation.Brands.EOS.Devices;
using Biovation.CommonClasses;
using Biovation.CommonClasses.Interface;
using Biovation.Constants;
using Biovation.Domain;
using Biovation.Service.Api.v2;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using RestSharp;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Biovation.Brands.EOS.Commands
{
    public class EosUserAdaptation : ICommand
    {

        /// <summary>
        /// All connected devices
        /// </summary>
        private readonly TaskTypes _taskTypes;
        private readonly RestClient _restClient;
        private readonly TaskService _taskService;
        private readonly UserService _userService;
        private readonly TaskStatuses _taskStatuses;
        private readonly DeviceService _deviceService;
        private readonly TaskItemTypes _taskItemTypes;
        private readonly TaskPriorities _taskPriorities;


        private Dictionary<uint, Device> OnlineDevices { get; }
        private Dictionary<uint, uint> EquivalentCodes { get; set; }
        private uint CreatorUserId { get; set; }
        private string Token { get; set; }
        //private long Code { get; set; }
        private TaskItem TaskItem { get; }
        public EosUserAdaptation(IReadOnlyList<object> items, Dictionary<uint, Device> devices, DeviceService deviceService, TaskTypes taskTypes, TaskService taskService,
            TaskStatuses taskStatuses, TaskItemTypes taskItemTypes, TaskPriorities taskPriorities, UserService userService, RestClient restClient)
        {
            TaskItem = (TaskItem)items[0];

            OnlineDevices = devices;
            _taskTypes = taskTypes;
            _restClient = restClient;
            _userService = userService;
            _taskService = taskService;
            _taskStatuses = taskStatuses;
            _deviceService = deviceService;
            _taskItemTypes = taskItemTypes;
            _taskPriorities = taskPriorities;
        }

        public object Execute()
        {
            if (TaskItem is null)
                return new ResultViewModel { Id = TaskItem?.Id ?? 0, Code = Convert.ToInt64(TaskStatuses.FailedCode), Message = $"Error in processing task item {TaskItem?.Id ?? 0}.{Environment.NewLine}", Validate = 0 };

            var deviceId = TaskItem.DeviceId;
            var device = _deviceService.GetDevice(deviceId).Result?.Data;

            if (device is null)
                return new ResultViewModel { Id = TaskItem.Id, Code = Convert.ToInt64(TaskStatuses.FailedCode), Message = $"Error in processing task item {TaskItem.Id}, wrong or zero device id is provided.{Environment.NewLine}", Validate = 0 };

            if (!OnlineDevices.ContainsKey(device.Code))
            {
                Logger.Log($"The device: {device.Code} is not connected.");
                return new ResultViewModel { Validate = 0, Id = deviceId, Code = Convert.ToInt64(TaskStatuses.DeviceDisconnectedCode) };
            }

            var data = (JObject)JsonConvert.DeserializeObject(TaskItem.Data);
            try
            {
                if (data != null)
                {
                    Token = Convert.ToString(data["token"]);
                    EquivalentCodes =
                        JsonConvert.DeserializeObject<Dictionary<uint, uint>>(Convert.ToString(data["serializedEquivalentCodes"]) ?? string.Empty);
                    CreatorUserId = Convert.ToUInt32(data["creatorUserId"]);
                }
            }
            catch (Exception e)
            {
                Logger.Log($"The Data of device {device.Code} is not valid.");
                Logger.Log(e, logType: LogType.Error);
                return new ResultViewModel { Success = false, Id = deviceId, Code = Convert.ToInt64(TaskStatuses.DeviceDisconnectedCode) };
            }

            var creatorUser = _userService.GetUsers(userId: CreatorUserId).Result?.Data?.Data?.FirstOrDefault();
            var onlineDevice = OnlineDevices.FirstOrDefault(dev => dev.Key == device.Code).Value;

            var restRequest = new RestRequest($"{device.ServiceInstance.Id}/Device/RetrieveUsersListFromDevice", Method.GET);
            restRequest.AddQueryParameter("code", device.Code.ToString());
            restRequest.ReadWriteTimeout = 3600000;
            restRequest.Timeout = 3600000;
            restRequest.AddHeader("Authorization", Token ?? string.Empty);

            var userList = _restClient.ExecuteAsync<ResultViewModel<List<User>>>(restRequest).Result.Data?.Data;
            if (userList is null)
                return new ResultViewModel { Success = false, Message = "The device is offline" };

            foreach (var userCode in EquivalentCodes.Keys.Where(userCode => userList.Any(user => user.Code == userCode)))
            {
                try
                {
                    var correctedUser = onlineDevice.GetUser(userCode);
                    if (correctedUser == null)
                    {
                        Logger.Log($"User {userCode}  doesn't exist on Device {device.DeviceId}");
                        return new ResultViewModel
                        {
                            Success = false,
                            Id = deviceId,
                            Code = Convert.ToInt64(TaskStatuses.FailedCode),
                            Message = $"User {userCode}  doesn't exist on Device {device.DeviceId}"
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
                        Data = JsonConvert.SerializeObject(new { userCode }),
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

                    correctedUser.Code = EquivalentCodes[userCode];

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
                }
                catch (Exception exception)
                {
                    Logger.Log(exception);
                    return new ResultViewModel { Validate = 0, Id = deviceId, Code = Convert.ToInt64(TaskStatuses.FailedCode) };
                }
            }

            restRequest = new RestRequest($"{device.ServiceInstance.Id}/Task/RunProcessQueue", Method.POST);
            _restClient.ExecuteAsync<ResultViewModel>(restRequest);

            return new ResultViewModel
            {
                Success = true,
                Id = deviceId,
                Code = Convert.ToInt64(TaskStatuses.DoneCode),
                Message = $"The Delete and send User operations for Users  on Device {device.DeviceId} successfully started"
            };
        }

        public void Rollback()
        {
            throw new NotImplementedException();
        }

        public string GetTitle()
        {
            return "Adapt users' Code.";
        }

        public string GetDescription()
        {
            return "Adapt users' Code.";
            //return $"Adapt users' code for device {Code}";
        }

    }
}
