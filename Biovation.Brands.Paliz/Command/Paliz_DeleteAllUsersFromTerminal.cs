using Biovation.CommonClasses;
using Biovation.Constants;
using Biovation.Domain;
using Biovation.Service.Api.v2;
using System;
using System.Collections.Generic;
using System.Linq;
using Newtonsoft.Json.Linq;
using Newtonsoft.Json;
using Biovation.CommonClasses.Interface;
using PalizTiara.Api.CallBacks;
using LogService = Biovation.Service.Api.v2.LogService;

namespace Biovation.Brands.Paliz.Command
{
    public class PalizDeleteAllUsersFromTerminal : ICommand
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
        private SetActionEventArgs _allUsersDeletionResult;
        public PalizDeleteAllUsersFromTerminal(IReadOnlyList<object> items, PalizServer palizServer, TaskService taskService, DeviceService deviceService,
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
                return new ResultViewModel
                {
                    Code = Convert.ToInt64(TaskStatuses.DeviceDisconnectedCode),
                    Id = TerminalId
                    ,
                    Message = $"The device: {Code} is not connected.",
                    Validate = 1
                };
            }

            try
            {
                _palizServer._serverManager.DeleteAllUsersEvent += DeleteAllUsersEventCallBack;
                _palizServer._serverManager.DeleteAllUsersAsyncTask(TerminalName);
                Logger.Log(GetDescription());

                // Waiting for the deletion result to get sent to the callback method.
                System.Threading.Thread.Sleep(500);
                while (_allUsersDeletionResult == null)
                {
                    System.Threading.Thread.Sleep(500);
                }

                _palizServer._serverManager.DeleteAllUsersEvent -= DeleteAllUsersEventCallBack;
                if (_allUsersDeletionResult.Result)
                {
                    Logger.Log($"  +Users successfully deleted from device: {Code}.\n");
                    return new ResultViewModel
                    {
                        Code = Convert.ToInt64(TaskStatuses.DoneCode),
                        Id = TerminalId,
                        Message = $"  +Users successfully deleted from device: {Code}.\n",
                        Validate = 1
                    };
                }
                Logger.Log($"  +Cannot delete all users from device: {Code}.\n");
                return new ResultViewModel
                {
                    Code = Convert.ToInt64(TaskStatuses.FailedCode),
                    Id = TerminalId,
                    Message = $"  +Cannot delete all users from device: {Code}.",
                    Validate = 0
                };

            }
            catch (Exception exception)
            {
                Logger.Log(exception);
                return new ResultViewModel { Code = Convert.ToInt64(TaskStatuses.FailedCode), Id = TerminalId, Message = $"Exception: {exception}", Validate = 0 };

            }
        }
        private void DeleteAllUsersEventCallBack(object sender, SetActionEventArgs args)
        {
            _allUsersDeletionResult = args;
            if (_allUsersDeletionResult.Result == false)
            {
                return;
            }

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
            return "Delete all users from terminal";
        }

        public string GetDescription()
        {
            return $"Deleting all users from device: {TerminalId}.";
        }
    }
}