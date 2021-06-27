using Biovation.Brands.Suprema.Devices;
using Biovation.CommonClasses;
using Biovation.CommonClasses.Interface;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Biovation.Constants;
using Biovation.Domain;
using Biovation.Service.Api.v2;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using DeviceService = Biovation.Service.Api.v2.DeviceService;

namespace Biovation.Brands.Suprema.Commands
{
    /// <summary>
    /// کنترل کننده برای تمامی اتفاقات بر روی تمامی و انواع مختلف ساعت ها
    /// </summary>
    public class SupremaSyncUserOfDevice : ICommand
    {

        /// <summary>
        /// All connected devices
        /// </summary>
        ///
        /// 
        private readonly Dictionary<uint, Device> _onlineDevices;
        private TaskItem TaskItem { get; }
        private readonly DeviceService _deviceService;

        private readonly AccessGroupService _accessGroupService;
        private readonly UserService _userService;

        public SupremaSyncUserOfDevice(TaskItem taskItem, Dictionary<uint, Device> onlineDevices, AccessGroupService accessGroupService, UserService userService, DeviceService deviceService)
        {
            TaskItem = taskItem;
            _onlineDevices = onlineDevices;

            _accessGroupService = accessGroupService;
            _userService = userService;
            _deviceService = deviceService;
        }

        /// <summary>
        /// <En>Handles the received event on devices.</En>
        /// <Fa>درخواست دریافت شده را کنترل میکند.</Fa>
        /// </summary>
        public object Execute()
        {

            if (TaskItem is null)
                return new ResultViewModel { Id = 0, Code = Convert.ToInt64(TaskStatuses.FailedCode), Message = $"Error in processing task item.{Environment.NewLine}", Validate = 0 };

            var deviceId = TaskItem.DeviceId;

            var parseResult = uint.TryParse(JsonConvert.DeserializeObject<JObject>(TaskItem.Data)?["UserId"]?.ToString() ?? "0", out var userId);

            if (!parseResult || userId == 0)
                return new ResultViewModel { Id = TaskItem.Id, Code = Convert.ToInt64(TaskStatuses.FailedCode), Message = $"Error in processing task item {TaskItem.Id}, zero or null user id is provided in data.{Environment.NewLine}", Validate = 0 };

            var deviceBasicInfo = _deviceService.GetDevice(deviceId).Result?.Data;
            if (deviceBasicInfo is null)
                return new ResultViewModel { Id = TaskItem.Id, Code = Convert.ToInt64(TaskStatuses.FailedCode), Message = $"Error in processing task item {TaskItem.Id}, wrong or zero device id is provided.{Environment.NewLine}", Validate = 0 };

            if (!_onlineDevices.ContainsKey(deviceBasicInfo.Code))
            {
                Logger.Log($"The device: {deviceBasicInfo.DeviceId} is not connected.");
                return new ResultViewModel { Validate = 0, Id = TaskItem.Id, Code = Convert.ToInt64(TaskStatuses.DeviceDisconnectedCode) };
            }


            var user = _userService.GetUsers(userId:userId).Result?.Data?.Data.FirstOrDefault();

            if (user == null)
            {
                return false;
            }


            var userAccess = _accessGroupService.GetAccessGroups(user.Id).Result?.Data?.Data;

            var fullAccess = userAccess.FirstOrDefault(ua => ua.Id == 254);
            var noAccess = userAccess.FirstOrDefault(ua => ua.Id == 253);
            var disable = userAccess.FirstOrDefault(ua => ua.Name.ToUpper() == "DISABLE");


            #region transferUserToDevice

            try
            {
                var device = _onlineDevices[Convert.ToUInt32(deviceBasicInfo.Code)];

                Task.Run(() =>
                {
                    if (device == null) return;
                    if (!device.TransferUser(user)) return;

                    Logger.Log($"User {user.Id} transferred to device {device.GetDeviceInfo().DeviceId} successfully.");
                });
            }
            catch (Exception)
            {
                return false;
            }

            #endregion transferUserToDevices

            return true;
        }

        public void Rollback()
        {
            throw new NotImplementedException();
        }

        public string GetTitle()
        {
            return "Sync all users of a device command";
        }

        public string GetDescription()
        {
            return "Syncing a users of a device command";
        }
    }
}