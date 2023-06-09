﻿using Biovation.CommonClasses;
using Biovation.CommonClasses.Interface;
using Biovation.Constants;
using Biovation.Domain;
using Biovation.Service.Api.v1;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using UCSAPICOMLib;

namespace Biovation.Brands.Virdi.Command
{
    public class VirdiDeleteUserFromTerminal : ICommand
    {
        /// <summary>
        /// All connected devices
        /// </summary>
        private Dictionary<uint, DeviceBasicInfo> OnlineDevices { get; }

        private int DeviceId { get; }
        private uint Code { get; }
        private int UserId { get; }

        private int TaskItemId { get; }

        private readonly ITerminalUserData _terminalUserData;
        private readonly LogService _logService;

        private readonly LogEvents _logEvents;
        private readonly LogSubEvents _logSubEvents;
        private readonly MatchingTypes _matchingTypes;

        public VirdiDeleteUserFromTerminal(IReadOnlyList<object> items, VirdiServer virdiServer, TaskService taskService, LogService logService, DeviceService deviceService, LogEvents logEvents, LogSubEvents logSubEvents, MatchingTypes matchingTypes, ITerminalUserData terminalUserData)
        {
            DeviceId = Convert.ToInt32(items[0]);

            TaskItemId = Convert.ToInt32(items[1]);
            var taskItem = taskService.GetTaskItem(TaskItemId);
            var data = (JObject)JsonConvert.DeserializeObject(taskItem.Data);

            UserId = (int)(data?["userCode"] ?? -1);
            Code = deviceService.GetDevices(brandId: DeviceBrands.VirdiCode).FirstOrDefault(d => d.DeviceId == DeviceId)?.Code ?? 0;
            OnlineDevices = virdiServer.GetOnlineDevices();

            _logService = logService;
            _logEvents = logEvents;
            _logSubEvents = logSubEvents;
            _matchingTypes = matchingTypes;
            _terminalUserData = terminalUserData;
        }
        public object Execute()
        {
            if (OnlineDevices.All(device => device.Key != Code))
            {
                Logger.Log($"DeleteUser,The device: {Code} is not connected.");
                return new ResultViewModel { Code = Convert.ToInt64(TaskStatuses.DeviceDisconnectedCode), Id = DeviceId, Message = $"The device: {Code} is not connected.", Validate = 1 };
            }

            try
            {
                lock (_terminalUserData)
                {
                    _terminalUserData.DeleteUserFromTerminal(TaskItemId, (int)Code, UserId);

                    Logger.Log("-->Delete user from terminal");

                    if (_terminalUserData.ErrorCode == 0)
                    {
                        Logger.Log($"  +User {UserId} successfully deleted from device: {Code}.\n");

                        var log = new Log
                        {
                            DeviceId = DeviceId,
                            LogDateTime = DateTime.Now,
                            EventLog = _logEvents.RemoveUserFromDevice,
                            UserId = UserId,
                            MatchingType = _matchingTypes.Unknown,
                            SubEvent = _logSubEvents.Normal,
                            TnaEvent = 0,
                            SuccessTransfer = true
                        };

                        _logService.AddLog(log);

                        return new ResultViewModel
                        {
                            Code = Convert.ToInt64(TaskStatuses.DoneCode),
                            Id = DeviceId,
                            Message = $"  +User {UserId} successfully deleted from device: {Code}.\n",
                            Validate = 1
                        };
                    }

                    Logger.Log($"  +Cannot delete user {UserId} from device: {Code}. Error code = {_terminalUserData.ErrorCode}\n");

                    return new ResultViewModel { Code = Convert.ToInt64(TaskStatuses.FailedCode), Id = DeviceId, Message = $"  +Cannot delete user {UserId} from device: {Code}. Error code = {_terminalUserData.ErrorCode}\n", Validate = 0 };
                }
            }
            catch (Exception exception)
            {
                Logger.Log(exception);
                return new ResultViewModel { Code = Convert.ToInt64(TaskStatuses.FailedCode), Id = DeviceId, Message = $"  +Cannot delete user {UserId} from device: {Code}. Error code = {_terminalUserData.ErrorCode}\n", Validate = 0 };
            }
        }

        public void Rollback()
        {
            throw new NotImplementedException();
        }

        public string GetTitle()
        {
            return "Delete user from terminal";
        }

        public string GetDescription()
        {
            return $"Deleting user: {UserId} from device: {DeviceId}.";
        }
    }
}
