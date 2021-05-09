using Biovation.CommonClasses;
using Biovation.Constants;
using Biovation.Domain;
using Biovation.Service.Api.v2;
using System;
using System.Collections.Generic;
using System.Linq;
using PalizTiara.Api.Models;
using Newtonsoft.Json.Linq;
using Newtonsoft.Json;
using Biovation.CommonClasses.Interface;
using PalizTiara.Api.CallBacks;
using LogService = Biovation.Service.Api.v2.LogService;

namespace Biovation.Brands.Paliz.Command
{
    public class PalizDeleteUserFromTerminal : ICommand
    {
        /// <summary>
        /// All connected devices
        /// </summary>
        private Dictionary<uint, DeviceBasicInfo> OnlineDevices { get; }

        private readonly LogService _logService;
        private readonly LogEvents _logEvents;
        private readonly LogSubEvents _logSubEvents;
        private int TaskItemId { get; }
        private string TerminalName { get; }
        private int TerminalId { get; }
        private uint Code { get; }
        private int UserId { get; }
        private readonly PalizServer _palizServer;
        private readonly MatchingTypes _matchingTypes;
        private UserActionEventArgs _userDeletionResult;
        public PalizDeleteUserFromTerminal(IReadOnlyList<object> items, PalizServer palizServer, TaskService taskService, DeviceService deviceService,
            LogService logService, LogEvents logEvents, LogSubEvents logSubEvents, MatchingTypes matchingTypes)
        {
            TerminalId = Convert.ToInt32(items[0]);
            TaskItemId = Convert.ToInt32(items[1]);
            var taskItem = taskService.GetTaskItem(TaskItemId)?.Data ?? new TaskItem();
            var data = (JObject)JsonConvert.DeserializeObject(taskItem.Data);
            if (data != null) UserId = (int)data["userCode"];
            _palizServer = palizServer;
            _logService = logService;
            _logEvents = logEvents;
            _logSubEvents = logSubEvents;
            _matchingTypes = matchingTypes;

            var devices = deviceService.GetDevices(brandId: DeviceBrands.PalizCode);
            if (devices is null)
            {
                OnlineDevices = new Dictionary<uint, DeviceBasicInfo>();
                return;
            }

            Code = devices.Data?.Data.FirstOrDefault(d => d.DeviceId == TerminalId)?.Code ?? 7;
            TerminalName = devices.Data?.Data.FirstOrDefault(d => d.DeviceId == TerminalId)?.Name ?? string.Empty;
            OnlineDevices = palizServer.GetOnlineDevices();
        }
        public object Execute()
        {
            if (OnlineDevices.All(device => device.Key != Code))
            {
                Logger.Log($"DeleteUser,The device: {Code} is not connected.");
                return new ResultViewModel { Code = Convert.ToInt64(TaskStatuses.DeviceDisconnectedCode), Id = TerminalId
                    , Message = $"The device: {Code} is not connected.", Validate = 1 };
            }

            try
            {
                _palizServer._serverManager.DeleteUserEvent += DeleteUserByIdEventCallBack;
                var userIdModel = new UserIdModel(UserId);
                _palizServer._serverManager.DeleteUserByIdAsyncTask(TerminalName, userIdModel);
                Logger.Log(GetDescription());

                // Waiting for the deletion result to get sent to the callback method.
                System.Threading.Thread.Sleep(500);
                while (_userDeletionResult == null)
                {
                    System.Threading.Thread.Sleep(500);
                }

                _palizServer._serverManager.DeleteUserEvent -= DeleteUserByIdEventCallBack;
                if (_userDeletionResult.Result)
                {
                    return new ResultViewModel
                    { 
                        Id = TerminalId,
                        Message = $"  +User {UserId} successfully deleted from device: {Code}.\n",
                        Validate = 1
                    };
                }
                return new ResultViewModel
                {
                    Code = Convert.ToInt64(TaskStatuses.FailedCode),
                    Id = TerminalId,
                    Message = $"  +Cannot delete user {UserId} from device: {Code}.",
                    Validate = 0
                };

            }
            catch (Exception exception)
            {
                Logger.Log(exception);
                return new ResultViewModel { Code = Convert.ToInt64(TaskStatuses.FailedCode), Id = TerminalId, Message = $"Exception: {exception}", Validate = 0 };

            }
        }
        private void DeleteUserByIdEventCallBack(object sender, UserActionEventArgs args)
        {
            _userDeletionResult = args;
            if (_userDeletionResult.Result == false)
            {
                Logger.Log($"  +Cannot delete user {UserId} from device: {Code}.\n");
                return;
            }
            Logger.Log($"  +User {UserId} successfully deleted from device: {Code}.\n");

            var log = new Log
            {
                DeviceId = TerminalId,
                LogDateTime = DateTime.Now,
                EventLog = _logEvents.RemoveUserFromDevice,
                UserId = UserId,
                MatchingType = _matchingTypes.Unknown,
                SubEvent = _logSubEvents.Normal,
                TnaEvent = 0,
                SuccessTransfer = true
            };

            _logService.AddLog(log);
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
            return $"Deleting user: {UserId} from device: {TerminalId}.";
        }
    }
}