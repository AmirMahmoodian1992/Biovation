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
    public class ZkRetrieveUsersListFromTerminal : ICommand
    {
        /// <summary>
        /// All connected devices
        /// </summary>
        private Dictionary<uint, Device> OnlineDevices { get; }
        private int DeviceId { get; }
        private uint Code { get; }
        private bool EmbedTemplate { get; }

        public ZkRetrieveUsersListFromTerminal(IReadOnlyList<object> items, Dictionary<uint, Device> devices, DeviceService deviceService)
        {
            OnlineDevices = devices;
            //DeviceId = Convert.ToInt32(items[0]);
            //TaskItemId = Convert.ToInt32(items[1]);
            var taskItem = (TaskItem)items[0];
            DeviceId = taskItem.DeviceId;
            Code = (deviceService.GetDevices(brandId: DeviceBrands.ZkTecoCode).FirstOrDefault(d => d.DeviceId == DeviceId)?.Code ?? 0);
            //DeviceId = devices.FirstOrDefault(dev => dev.Key == Code).Value.GetDeviceInfo().DeviceId;
            //var taskItem = taskService.GetTaskItem(TaskItemId);
            var data = (JObject)JsonConvert.DeserializeObject(taskItem.Data);
            if (data != null) EmbedTemplate = Convert.ToBoolean(data["embedTemplate"]);
        }
        public object Execute()
        {
            if (OnlineDevices.All(device => device.Key != Code))
            {
                Logger.Log($"The device: {Code} is not connected.");
                return new ResultViewModel<List<User>>
                {
                    Id = DeviceId,
                    Data = new List<User>(),
                    Success = true,
                    Code = Convert.ToInt64(TaskStatuses.DeviceDisconnectedCode)
                };
            }

            try
            {
                var device = OnlineDevices.FirstOrDefault(dev => dev.Key == Code).Value;
                var result = device.GetAllUserInfo(EmbedTemplate);
                return new ResultViewModel<List<User>> { Data = result, Id = DeviceId, Message = "0", Validate = 1, Code = Convert.ToInt64(TaskStatuses.DoneCode) };

            }
            catch (Exception exception)
            {
                Logger.Log(exception);
                return new ResultViewModel<List<User>>
                {
                    Id = DeviceId,
                    Data = new List<User>(),
                    Code = Convert.ToInt64(TaskStatuses.FailedCode),
                    Success = false
                };
            }
        }

        public void Rollback()
        {
            throw new NotImplementedException();
        }

        public string GetTitle()
        {
            return "Retrieve all users from terminal";
        }

        public string GetDescription()
        {
            return $"Retrieving all users from device: {Code}.";
        }
    }
}
