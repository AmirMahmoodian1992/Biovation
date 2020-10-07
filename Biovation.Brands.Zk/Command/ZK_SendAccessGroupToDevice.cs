using Biovation.Brands.ZK.Devices;
using Biovation.CommonClasses;
using Biovation.CommonClasses.Interface;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using Biovation.Constants;
using Biovation.Domain;
using Biovation.Service.Api.v1;

namespace Biovation.Brands.ZK.Command
{
    public class ZkSendAccessGroupToDevice : ICommand
    {
        /// <summary>
        /// All connected devices
        /// </summary>
        private Dictionary<uint, Device> OnlineDevices { get; }

        private int DeviceId { get; }
        private int TaskItemId { get; }
        private uint Code { get; }
        private int AccessGroupId { get; }
        private AccessGroup AccessGroupObj { get; }

        public ZkSendAccessGroupToDevice(IReadOnlyList<object> items, Dictionary<uint, Device> devices, DeviceService deviceService, TaskService taskService, AccessGroupService accessGroupService)
        {
            OnlineDevices = devices;

            DeviceId = Convert.ToInt32(items[0]);
            TaskItemId = Convert.ToInt32(items[1]);
            Code = (deviceService.GetDevices(brandId: DeviceBrands.ZkTecoCode).FirstOrDefault(d => d.DeviceId == DeviceId)?.Code ?? 0);

            var taskItem = taskService.GetTaskItem(TaskItemId);
            var data = (JObject)JsonConvert.DeserializeObject(taskItem.Data);
            AccessGroupId = (int)(data["accessGroupId"]);

            AccessGroupObj = accessGroupService.GetAccessGroup(AccessGroupId);
        }

        public object Execute()
        {
            if (OnlineDevices.All(device => device.Key != Code))
            {
                Logger.Log($"The device: {Code} is not connected.");
                return false;
            }

            if (AccessGroupObj == null)
            {
                Logger.Log($"Access Group {AccessGroupId} does not exist.");
                return false;
            }

            try
            {
                var device = OnlineDevices.FirstOrDefault(dev => dev.Key == Code).Value;
                var result = device.TransferAccessGroup(AccessGroupObj);
                return result;
            }
            catch (Exception exception)
            {
                Logger.Log(exception);
            }

            return false;
        }

        public void Rollback()
        {
            throw new NotImplementedException();
        }

        public string GetTitle()
        {
            return "Send access group to terminal";
        }

        public string GetDescription()
        {
            return $"Sending access group: {AccessGroupId} to device: {Code}.";
        }
    }
}
