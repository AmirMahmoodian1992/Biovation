﻿using Biovation.Brands.EOS.Devices;
using Biovation.CommonClasses;
using Biovation.CommonClasses.Interface;
using Biovation.Constants;
using Biovation.Domain;
using Biovation.Service.Api.v2;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Biovation.Brands.Eos.Commands
{
    /// <summary>
    /// کنترل کننده برای تمامی اتفاقات بر روی تمامی و انواع مختلف ساعت ها
    /// </summary>
    public class EosSendUserToDevice : ICommand
    {
        //private User User { get; }
        //private uint _deviceId { get; }
        //private uint _userId { get; }
        private TaskItem TaskItem { get; }

        private readonly UserService _userService;
        private readonly DeviceService _deviceService;
        private readonly AdminDeviceService _adminDeviceService;

        /// <summary>
        /// All connected devices
        /// </summary>
        private Dictionary<uint, Device> OnlineDevices { get; }

        public EosSendUserToDevice(TaskItem taskItem, Dictionary<uint, Device> onlineDevices, UserService userService, DeviceService deviceService, AdminDeviceService adminDeviceService)
        {
            //_userId = userId;
            //_deviceId = deviceId;
            _userService = userService;
            _deviceService = deviceService;
            _adminDeviceService = adminDeviceService;

            TaskItem = taskItem;
            OnlineDevices = onlineDevices;
            //User = _userService.GetUsers(code: userId)?.Data?.Data.FirstOrDefault();
        }

        /// <summary>
        /// <En>Handles the received event on devices.</En>
        /// <Fa>درخواست دریافت شده را کنترل میکند.</Fa>
        /// </summary>
        public object Execute()
        {
            if (TaskItem is null)
                return new ResultViewModel { Id = TaskItem.Id, Code = Convert.ToInt64(TaskStatuses.FailedCode), Message = $"Error in processing task item {TaskItem.Id}.{Environment.NewLine}", Validate = 0 };

            var deviceId = TaskItem.DeviceId;
            var parseResult = uint.TryParse(JsonConvert.DeserializeObject<JObject>(TaskItem.Data)?["userId"].ToString() ?? "0", out var userId);

            if (!parseResult || userId == 0)
                return new ResultViewModel { Id = TaskItem.Id, Code = Convert.ToInt64(TaskStatuses.FailedCode), Message = $"Error in processing task item {TaskItem.Id}, zero or null user id is provided in data.{Environment.NewLine}", Validate = 0 };

            var device = _deviceService.GetDevice(deviceId)?.Data;
            if (device is null)
                return new ResultViewModel { Id = TaskItem.Id, Code = Convert.ToInt64(TaskStatuses.FailedCode), Message = $"Error in processing task item {TaskItem.Id}, wrong or zero device id is provided.{Environment.NewLine}", Validate = 0 };

            if (!OnlineDevices.ContainsKey(device.Code))
                return new ResultViewModel { Id = TaskItem.Id, Code = Convert.ToInt64(TaskStatuses.DeviceDisconnectedCode), Message = $"  Enroll User face from device: {device.Code} failed. The device is disconnected.{Environment.NewLine}", Validate = 0 };

            var user = _userService.GetUsers(userId: userId)?.Data?.Data.FirstOrDefault();

            if (user == null)
            {
                Logger.Log($"User {userId} does not exist.");
                return new ResultViewModel { Validate = 0, Id = TaskItem.Id, Message = $"User {userId} does not exist.", Code = Convert.ToInt64(TaskStatuses.DeviceDisconnectedCode) };
            }

            try
            {
                var onlineDevice = OnlineDevices.FirstOrDefault(dev => dev.Key == device.DeviceId).Value;
                var adminDevices = _adminDeviceService.GetAdminDevicesByUserId((int)user.Code)?.Data?.Data;
                user.IsAdmin = adminDevices?.Any(x => x.DeviceId == device.DeviceId) ?? false;
                var result = onlineDevice.TransferUser(user);

                return new ResultViewModel { Validate = result ? 1 : 0, Id = device.DeviceId, Message = $"User {user.Id} Send", Code = Convert.ToInt64(TaskStatuses.DoneCode) };
            }
            catch (Exception exception)
            {
                Logger.Log(exception);
                return new ResultViewModel { Validate = 0, Id = device.DeviceId, Message = exception.Message, Code = Convert.ToInt64(TaskStatuses.FailedCode) };
            }
        }

        public void Rollback()
        {
            throw new NotImplementedException();
        }

        public string GetTitle()
        {
            return "Sync a user with all devices command";
        }

        public string GetDescription()
        {
            //return "Syncing User (id: " + User.Id + " name: " + User.UserName + ") command";
            return "Sync a user with all devices command";
        }
    }
}