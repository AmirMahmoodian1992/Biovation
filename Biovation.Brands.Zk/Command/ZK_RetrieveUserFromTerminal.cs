using Biovation.Brands.ZK.Devices;
using Biovation.CommonClasses;
using Biovation.CommonClasses.Interface;
using Biovation.Constants;
using Biovation.Domain;
using Biovation.Service.Api.v1;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Biovation.Brands.ZK.Command
{
    public class ZkRetrieveUserFromTerminal : ICommand
    {
        /// <summary>
        /// All connected devices
        /// </summary>
        private Dictionary<uint, Device> OnlineDevices { get; }

        private int DeviceId { get; }
        private int UserId { get; }
        private bool Saving { get; }
        private uint Code { get; }
        private int TaskItemId { get; }
        private readonly TaskService _taskService;
        private readonly DeviceService _deviceService;

        public ZkRetrieveUserFromTerminal(IReadOnlyList<object> items, Dictionary<uint, Device> devices, TaskService taskService, DeviceService deviceService)
        {
            OnlineDevices = devices;
            _taskService = taskService;
            _deviceService = deviceService;

            DeviceId = Convert.ToInt32(items[0]);
            TaskItemId = Convert.ToInt32(items[1]);
            Code = (_deviceService.GetDevices(brandId: DeviceBrands.ZkTecoCode).FirstOrDefault(d => d.DeviceId == DeviceId)?.Code ?? 0);
            var taskItem = _taskService.GetTaskItem(TaskItemId);
            var data = (JObject)JsonConvert.DeserializeObject(taskItem.Data);
            if (data != null)
            {
                UserId = (int)data["userId"];
                if (data.ContainsKey("saving"))
                    Saving = (bool)data["saving"];
                else
                    Saving = true;
            }



            DeviceId = devices.FirstOrDefault(dev => dev.Key == Code).Value.GetDeviceInfo().DeviceId;
        }
        public object Execute()
        {
            if (OnlineDevices.All(device => device.Key != Code))
            {
                Logger.Log($"The device: {Code} is not connected.");
                return new ResultViewModel { Validate = 0, Id = DeviceId, Message = $"The device: {Code} is not connected.", Code = Convert.ToInt64(TaskStatuses.DeviceDisconnectedCode) };
            }


            try
            {
                var device = OnlineDevices.FirstOrDefault(dev => dev.Key == Code).Value;
                if (Saving)
                {
                    var result = device.GetAndSaveUser(UserId);
                    return new ResultViewModel
                    { Validate = result ? 1 : 0, Id = DeviceId, Code = Convert.ToInt64(TaskStatuses.DoneCode) };
                }
                else
                {
                    var result = device.GetUser(UserId);
                    return new ResultViewModel<User>
                    { Success = result is { }, Data = result, Id = DeviceId, Code = Convert.ToInt64(TaskStatuses.DoneCode) };
                }
            }
            catch (Exception exception)
            {
                Logger.Log(exception);
                if (Saving)
                {
                    return new ResultViewModel
                    {
                        Success = false,
                        Id = DeviceId,
                        Message = exception.Message,
                        Code = Convert.ToInt64(TaskStatuses.FailedCode)
                    };
                }
                return new ResultViewModel<User>
                {
                    Success = false,
                    Id = DeviceId,
                    Message = exception.Message,
                    Code = Convert.ToInt64(TaskStatuses.FailedCode)
                };
            }
        }

        public void Rollback()
        {
            throw new NotImplementedException();
        }

        public string GetTitle()
        {
            return "Retrieve user from terminal";
        }

        public string GetDescription()
        {
            return $"Retrieving user: {UserId} from device: {Code}.";
        }
    }
}
